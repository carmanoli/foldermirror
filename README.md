# foldermirror
Mirror one folder to another

### Prerequisites
To compile this project you will need [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later.

### Compilation and Execution 
You can use Visual Studio or using the command line.

#### Clone the repository
```bash
git clone https://github.com/carmanoli/foldermirror.git
cd foldermirror
```

#### Compile
```bash
dotnet build 
```

#### Run
```bash
cd bin/Release/net8.0/
dotnet foldermirror.dll --source <source_folder> --target <destination_folder> [--logfile <lofile_name_>] [--interval <seconds>]
```


## Algorithm

e devolver
uma lista de pastas
uma lista de ficheiros
pastas existem 3 situações:
a pasta existe no destino mas não na origem: apaga
apasta existe na origem e não no destino copia a pasta toda
a pasta existe nos 2 sitios: chama a função recursivamente
ficheiros existem 3 situações:
a ficheiros existe no destino mas não na origem: apaga
ficheiros  existe na origem e não no destino copia o ficheiro
o ficheiro existe nos 2 sitios: calcula o md5 e compara, se for diferente copia