using Autofac;
using Autofac.Core;
using Autofac.Core.Lifetime;
using Autofac.Core.Resolving;
using Autofac.Util;
using MSBeta.Dapper.Grpc5.Tests.Core.Multitenant;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace MSBeta.Dapper.Grpc5.Tests.Core.Infrastructure
{
    public class MultitenantContainer : Disposable, IContainer
    {
        #region Fields
        private readonly object _defaultTenantId = "MSBeta_Default_TenantId";
        internal static readonly object TenantLifetimeScopeTag = "MSBeate_Tenant_Lifetime";

        private readonly System.Threading.ReaderWriterLockSlim _readWriteLock = new System.Threading.ReaderWriterLockSlim();

        private readonly Dictionary<object, ILifetimeScope> _tenantLifetimeScopes = new Dictionary<object, ILifetimeScope>();
        #endregion

        #region Ctor
        public MultitenantContainer(ITenantIdentificationStrategy tenantIdentificationStrategy, IContainer applicationContainer)
        {
            this.TenantIdentificationStrategy = tenantIdentificationStrategy
                ?? throw new ArgumentNullException(nameof(tenantIdentificationStrategy));

            this.ApplicationContainer = applicationContainer
                ?? throw new ArgumentNullException(nameof(applicationContainer));
        }
        #endregion

        #region Properties
        public ITenantIdentificationStrategy TenantIdentificationStrategy { get; private set; }
        public IContainer ApplicationContainer { get; private set; }
        #endregion

        #region Get Scope methods
        public ILifetimeScope GetCurrentTenantScope()
        {
            var tenantId = (object)null;
            if (this.TenantIdentificationStrategy.TryIdentifyTenant(out tenantId))
            {
                return this.GetTenantScope(tenantId);
            }

            return this.GetTenantScope(null);
        }

        public ILifetimeScope GetTenantScope(object tenantId)
        {
            if (tenantId == null)
            {
                tenantId = this._defaultTenantId;
            }

            var tenantScope = (ILifetimeScope)null;
            _readWriteLock.EnterReadLock();
            try
            {
                this._tenantLifetimeScopes.TryGetValue(tenantId, out tenantScope);
            }
            finally
            {
                _readWriteLock.ExitReadLock();
            }

            if (tenantScope == null)
            {
                _readWriteLock.EnterWriteLock();

                try
                {
                    if (!this._tenantLifetimeScopes.TryGetValue(tenantId, out tenantScope) || tenantScope == null)
                    {
                        tenantScope = this.ApplicationContainer.BeginLifetimeScope(TenantLifetimeScopeTag);
                        this._tenantLifetimeScopes[tenantId] = tenantScope;
                    }
                }
                finally
                {
                    _readWriteLock.ExitWriteLock();
                }
            }

            return tenantScope;
        }
        #endregion

        #region Tenant's methods ( Get / Remove / Configure & Register )
        public IEnumerable<object> GetTenants()
        {
            _readWriteLock.EnterReadLock();
            try
            {
                return new List<object>(_tenantLifetimeScopes.Keys);
            }
            finally
            {
                _readWriteLock.ExitReadLock();
            }
        }

        public void ConfigureTenant(object tenantId, Action<ContainerBuilder> configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (tenantId == null)
            {
                tenantId = this._defaultTenantId;
            }

            _readWriteLock.EnterUpgradeableReadLock();
            try
            {
                if (this._tenantLifetimeScopes.ContainsKey(tenantId))
                {
                    throw new InvalidOperationException(string.Format("该 tenantId:'{0}' 已经配置，不可以重复配置！！", tenantId));
                }

                _readWriteLock.EnterWriteLock();
                try
                {
                    this._tenantLifetimeScopes[tenantId] = this.ApplicationContainer.BeginLifetimeScope(TenantLifetimeScopeTag, configuration);
                }
                finally
                {
                    _readWriteLock.ExitWriteLock();
                }
            }
            finally
            {
                _readWriteLock.ExitUpgradeableReadLock();
            }
        }

        public bool RemoveTenant(object tenantId)
        {
            if (tenantId == null)
            {
                tenantId = this._defaultTenantId;
            }

            _readWriteLock.EnterWriteLock();
            try
            {
                if (this._tenantLifetimeScopes.TryGetValue(tenantId, out var tenantScope) && tenantScope != null)
                {
                    tenantScope.Dispose();

                    return _tenantLifetimeScopes.Remove(tenantId);
                }

                return false;
            }
            finally
            {
                _readWriteLock.ExitWriteLock();
            }
        }

        public void ClearTenants()
        {
            _readWriteLock.EnterWriteLock();
            try
            {
                foreach (var tenantScope in _tenantLifetimeScopes.Values)
                {
                    tenantScope.Dispose();
                }

                _tenantLifetimeScopes.Clear();
            }
            finally
            {
                _readWriteLock.ExitWriteLock();
            }
        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _readWriteLock.EnterWriteLock();

                try
                {
                    foreach (var scope in this._tenantLifetimeScopes.Values)
                    {
                        scope.Dispose();
                    }

                    this.ApplicationContainer.Dispose();
                }
                finally
                {
                    _readWriteLock.ExitWriteLock();
                }

                _readWriteLock.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Implement the interface properties
        public IDisposer Disposer => this.GetCurrentTenantScope().Disposer;

        public object Tag => this.GetCurrentTenantScope().Tag;

        public IComponentRegistry ComponentRegistry => this.GetCurrentTenantScope().ComponentRegistry;

        #endregion

        #region Implement the interface methods 

        public event EventHandler<LifetimeScopeBeginningEventArgs> ChildLifetimeScopeBeginning
        {
            add { this.GetCurrentTenantScope().ChildLifetimeScopeBeginning += value; }

            remove { this.GetCurrentTenantScope().ChildLifetimeScopeBeginning -= value; }
        }

        public event EventHandler<LifetimeScopeEndingEventArgs> CurrentScopeEnding
        {
            add { this.GetCurrentTenantScope().CurrentScopeEnding += value; }

            remove { this.GetCurrentTenantScope().CurrentScopeEnding -= value; }
        }

        public event EventHandler<ResolveOperationBeginningEventArgs> ResolveOperationBeginning
        {
            add { this.GetCurrentTenantScope().ResolveOperationBeginning += value; }

            remove { this.GetCurrentTenantScope().ResolveOperationBeginning -= value; }
        }

        public ILifetimeScope BeginLifetimeScope()
        {
            return this.GetCurrentTenantScope().BeginLifetimeScope();
        }

        public ILifetimeScope BeginLifetimeScope(object tag)
        {
            return this.GetCurrentTenantScope().BeginLifetimeScope(tag);
        }

        public ILifetimeScope BeginLifetimeScope(Action<ContainerBuilder> configurationAction)
        {
            return this.GetCurrentTenantScope().BeginLifetimeScope(configurationAction);
        }

        public ILifetimeScope BeginLifetimeScope(object tag, Action<ContainerBuilder> configurationAction)
        {
            return this.GetCurrentTenantScope().BeginLifetimeScope(tag, configurationAction);
        }

        public object ResolveComponent(IComponentRegistration registration, IEnumerable<Parameter> parameters)
        {
            return this.GetCurrentTenantScope().ResolveComponent(registration, parameters);
        }
        #endregion
    }
}
