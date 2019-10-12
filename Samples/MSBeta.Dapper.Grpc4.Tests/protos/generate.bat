@echo  

	SET TOOLS_PATH=tools\windows_x64
	%TOOLS_PATH%\protoc.exe -I. --csharp_out build SurfService.proto --grpc_out build --plugin=protoc-gen-grpc=%TOOLS_PATH%\grpc_csharp_plugin.exe
@echo  
@pause