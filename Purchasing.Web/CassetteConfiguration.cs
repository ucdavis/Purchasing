using Cassette.Configuration;
using Cassette.Scripts;
using Cassette.Stylesheets;
using System.Text.RegularExpressions;

namespace Purchasing.Web
{
    /// <summary>
    /// Configures the Cassette asset modules for the web application.
    /// </summary>
    public class CassetteConfiguration : ICassetteConfiguration
    {
        public void Configure(BundleCollection bundles, CassetteSettings settings)
        {
            //settings.IsDebuggingEnabled = false;
            // TODO: Configure your bundles here...
            // Please read http://getcassette.net/documentation/configuration

            // This default configuration treats each file as a separate 'bundle'.
            // In production the content will be minified, but the files are not combined.
            // So you probably want to tweak these defaults!
            //bundles.AddPerIndividualFile<StylesheetBundle>("Css");
            //bundles.AddPerIndividualFile<ScriptBundle>("Scripts");

            // To combine files, try something like this instead:
            //   bundles.Add<StylesheetBundle>("Content");
            // In production mode, all of ~/Content will be combined into a single bundle.
            //TODO: combine CSS into a bundle
            bundles.AddPerSubDirectory<StylesheetBundle>("Css", new FileSearch {Exclude = new Regex("/single/")});
            bundles.AddPerIndividualFile<StylesheetBundle>("Css/single");

            bundles.AddPerSubDirectory<ScriptBundle>("Scripts/external"); //just the CDN hosted stuff
            bundles.AddPerSubDirectory<ScriptBundle>("Scripts/public", new FileSearch {Exclude = new Regex("/single/")});
            bundles.AddPerIndividualFile<ScriptBundle>("Scripts/public/single"); //stuff in single is shared and can be referenced individually
            // If you want a bundle per folder, try this:
            //   bundles.AddPerSubDirectory<ScriptBundle>("Scripts");
            // Each immediate sub-directory of ~/Scripts will be combined into its own bundle.
            // This is useful when there are lots of scripts for different areas of the website.

            // *** TOP TIP: Delete all ".min.js" files now ***
            // Cassette minifies scripts for you. So those files are never used.
        }
    }
}