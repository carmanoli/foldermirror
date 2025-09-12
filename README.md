# foldermirror
Mirrors unidirectionally one folder to another, periodically and logs the operations.

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
foldermirror --source <source_folder> --target <destination_folder> [--logfile <lofile_name_>] [--interval <seconds>]
```


