# SpaceSavior

## What is this?

_SpaceSavior_ is the wave of the future in parking technologies! We're ready to 
take the world by storm with our upcoming suite of open-source and revolutionary
APIs!

These APIs coming soon to a cloud near you:

- ParkingProtagonist
- CarportCaptain
- GarageGenius
- LotLuminary

## What is it really?

This is just an ASP.NET Core application. It uses Swashbuckle to document the 
available endpoints and [AppMetrics](https://github.com/alhardy/AppMetrics)
to provide simple performance metrics.

It includes Docker support for running the application in a container.

### How to run it

If you have Visual Studio 2017, open the solution and press F5.
Your browser should open and navigate to the Swagger UI showing
the endpoints available for use.

### Metrics endpoints

Unfortunately there's no integration between Swashbuckle and AppMetrics, so
the AppMetrics endpoints do not currently appear on the Swagger UI. You can
access the AppMetrics endpoints using the following relative paths:

- `/appmetrics/metrics` - Exposes environment information and performance metrics
 about the application. The `timers` property of the "Application.HttpRequests"
 context includes statistics on response times.
- `/appmetrics/environment` - Exposes only environment information.
- `/appmetrics/health` - Runs a health check. This is currently configured to 
 check if the instance can ping google.com within one second.

The above paths can be changed using the appsettings.json configuration file.

### Changing rate information

The default rates can be changed by modifying the existing "rates.json" file or 
by changing the configured path in the appsettings.json configuration file
to use a different JSON input file on startup. Rates can also be changed 
using an endpoint while the application is running.

### Performance notes

Rate quotes are currently retrieved in O(n) time where n is the number of
rate definitions for a given day. For a very large number of rate definitions
on any given day, this may perform unacceptably.

One alternative is to use an 
[interval tree](https://en.wikipedia.org/wiki/Interval_tree), which would
make it possible to perform datetime range lookups in O(log n) time. I tried
using a package for this but couldn't find a straightforward way to configure it
to search for datetimes inclusively at the start of the range and exclusively at
the end of the range. 

An even faster alternative is to use a hash table and map each time to a rate.
Since rate quotes are only accurate to the minute, and not the second, our hash
table would only need a maximum of 10,080 keys (i.e. 60 minutes/hour * 24 hours/day * 
7 days/week = 10,080 minutes/week). Since we can map to within this space directly
from the time within the week, we don't need to worry about hash functions and 
collisions; we could build a 10,080 element array that maps minutes directly to the 
rate. For example, index 14 in the array would map to Sunday at 12:15 AM, and the 
index 1454 would map to Monday at 12:15 AM. It would be a fun experiment to see
how much faster that would be over the current O(n) implementation.

### Implementation notes

`dotnet publish` does not seem to support copying XML documentation files to
the published application's directory. These are normally only used in development,
but Swashbuckle can use XML documentation comments to populate text in its UI. In
order to get it to copy so that it would actually use this file when deployed in a
Docker container, I needed to modify the application's .csproj file to explicitly
copy that one file, like this:

```
<Target Name="PrepublishScript" BeforeTargets="PrepareForPublish">
  <ItemGroup>
    <DocFile Include="bin\**\**\SpaceSavior.Api.xml" />
  </ItemGroup>
  <Copy SourceFiles="@(DocFile)" DestinationFolder="$(PublishDir)" SkipUnchangedFiles="false" />
</Target>
```
