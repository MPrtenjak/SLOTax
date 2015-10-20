@setlocal

@cd SLOTaxGuiTest40
@msbuild /t:build /p:Configuration=Release /nologo /v:m
@if %errorlevel% neq 0 exit /b %errorlevel%
@cd ..

@cd SLOTaxGuiTest
@msbuild /t:build /p:Configuration=Release /nologo /v:m
@if %errorlevel% neq 0 exit /b %errorlevel%
@cd ..

@rd FinalOutput /s /q
@md FinalOutput
@cd FinalOutput
@md Net40
@md Net45

@cd ..

@copy SLOTaxGuiTest40\bin\Release\*.exe FinalOutput\Net40
@copy SLOTaxGuiTest40\bin\Release\*.dll FinalOutput\Net40
@copy SLOTaxGuiTest40\bin\Release\*.config FinalOutput\Net40
@del FinalOutput\Net40\*vshost*

@copy SLOTaxGuiTest\bin\Release\*.exe FinalOutput\Net45
@copy SLOTaxGuiTest\bin\Release\*.dll FinalOutput\Net45
@copy SLOTaxGuiTest\bin\Release\*.config FinalOutput\Net45
@del FinalOutput\Net45\*vshost*
