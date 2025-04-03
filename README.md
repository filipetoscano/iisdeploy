iisknife
=========================================================================

.NET 4.8 tool to ensure IIS configuration while xcopy'ing applications.


Usage
-------------------------------------------------------------------------

```bash
> iisknife
1.0.0

Usage: iisknife [command] [options]

Options:
  --version     Show version information.
  -?|-h|--help  Show help information.

Commands:
  apply         Applies IIS configuration
  deploy        Deploys files from source to target directories
  get           Views the current IIS configuration
  state         For blue/green deployments, gets the current / next versions

Run 'iisknife [command] -?|-h|--help' for more information about a command.
```

The `get` command is used to retrieve the current IIS configuration
in JSON.

The `apply` command is used to apply the IIS configuration (as per JSON)
file to the local IIS server. All managed settings will be test/setted.

The `deploy` command is used to xcopy from a (set of) source directory/ies
to the target location. This command requires both the JSON configuration
file and a source/target mapping.


Sample config file
-------------------------------------------------------------------------

```json
{
  "Name": "mydeploy",
  "RootPhysicalPath": "c:/appdir",
  "HasBlueGreen": false,
  "Sites": [
    {
      "Name": "website1",
      "AutoStart": true,
      "Bindings": [
        {
          "Protocol": "HTTP",
          "Port": 81
        }
      ],
      "Applications": [
        {
          "Path": "/",                         -- Website
          "PhysicalPath": "./site-app",
          "ApplicationPool": {
            "Name": "website_pool",
            "AutoStart": true,
            "ManagedPipelineMode": "Integrated",
            "ManagedRuntimeVersion": "v4.0",
            "ProcessModel": {
              "IdentityType": "ApplicationPoolIdentity"
            }
          },
          "VirtualDirectories": [
            {
              "Path": "/vdir1",                -- Vdir under website
              "PhysicalPath": "./site-vdir"
            },
          ]
        }
      ]
    }
  ]
}
```


Sample map file
-------------------------------------------------------------------------

```json
{
  "RootSource": "c:/AppSoftware",
  "Source": {
    "website1/": "./site-app",             -- Website
    "website1//vdir1": "./site-vdir",      -- Vdir under website
    "website1/webapp": "./web-app",        -- Web app
    "website1/webapp/vdir2": "./web-vdir", -- Vdir under webapp
  }
}
```


Option files
-------------------------------------------------------------------------

The `apply` and `deploy` commands take an `--options` flag, which will
load command options from a JSON file. The options are:

Apply options:

```json
{
  "RemoveUnmanagedSites": true | false,
  "RemoveUnmanagedApplications": true | false,
  "RemoveUnmanagedVdirs": true | false,
  "RemoveUnusedApplicationPools": true | false,
  "StartStopManagedApplicationPools": true | false,
  "RecycleManagedApplicationPools": true | false
}
```


Deploy options:

```json
{
  "StartStopManagedApplicationPools": true | false
}
```

By default, all the values are set to True.
