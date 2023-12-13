using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using nntrain.web;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();



//protected override async Task OnInitializedAsync()
//{
//    forecasts = await Http.GetFromJsonAsync<WeatherForecast[]>("sample-data/weather.json");
//}


//@foreach(var forecast in forecasts)
//{
//    < tr >
//        < td > @forecast.Date.ToShortDateString() </ td >
//        < td > @forecast.TemperatureC </ td >
//        < td > @forecast.TemperatureF </ td >
//        < td > @forecast.Summary </ td >
//    </ tr >
//}