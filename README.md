# Configuration.Expander Extension

[![Nuget](https://img.shields.io/nuget/v/Configuration.Expander?style=flat-square)](https://www.nuget.org/packages/Configuration.Expander)

An extension to enable configuration variable expansion. It depends on [Microsoft.Extensions.Configuration](https://www.nuget.org/packages/Microsoft.Extensions.Configuration) and [Microsoft.Extensions.Configuration.Binder](https://www.nuget.org/packages/Microsoft.Extensions.Configuration.Binder/).

## Usage

In any declaration you can use a variable expansion like `${ <VARIABLE_NAME> }`. The `<VARIABLE_NAME>` will be expandaded (replaced) by any value which key is defined in the `Configuration` object. A fallback value can be provided for the abscense of a variable/configuration `${ <VARIABLE_NAME> ?? <DEFAULT_VALUE> }`

#### Declaration
```json
{
    "db": {
        "host": "${ DB_HOST ?? localhost }"
    }
}
```

#### Usage in code
```csharp
configuration.ResolveValue<String>("db:host")
```

