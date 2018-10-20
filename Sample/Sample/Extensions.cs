using System;
using System.Threading.Tasks;
using Prism.Navigation;


namespace Sample
{
    public static class Extensions
    {
        public static async Task Navigate(this INavigationService navigation, string uri, params (string, object)[] tuples)
        {
            var parameters = new NavigationParameters();
            foreach (var p in tuples)
                parameters.Add(p.Item1, p.Item2);

            var result = await navigation.NavigateAsync(uri, parameters);
            if (!result.Success)
                throw new ArgumentException("Failed to Navigate", result.Exception);
        }
    }
}
