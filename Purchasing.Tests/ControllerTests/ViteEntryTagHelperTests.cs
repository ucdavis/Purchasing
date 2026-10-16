using System.Collections.Generic;
using System.IO;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Purchasing.Mvc.TagHelpers;

namespace Purchasing.Tests.ControllerTests
{
    [TestClass]
    public class ViteEntryTagHelperTests
    {
        [TestMethod]
        public void DevelopmentStylesLoadWithoutReactAndShareTheViteClient()
        {
            var view = new ViewContext { HttpContext = new DefaultHttpContext() };
            var css = Render("src/styles.css", "Development", true, null, view);
            StringAssert.Contains(css, "rel=\"stylesheet\"");
            StringAssert.Contains(css, "href=\"http://localhost:5178/src/styles.css\"");
            StringAssert.Contains(css, "@vite/client");
            Assert.IsFalse(css.Contains("@react-refresh"));
            Assert.AreEqual("", Render("src/styles.css", "Development", true, null, view));
            var script = Render("src/shell.tsx", "Development", true, null, view, react: true);
            StringAssert.Contains(script, "@react-refresh");
            Assert.IsFalse(script.Contains("@vite/client"));
        }

        [TestMethod]
        public void DevelopmentEntriesShareOneRefreshRuntime()
        {
            var view = new ViewContext { HttpContext = new DefaultHttpContext() };
            var first = Render("src/shell.tsx", "Development", true, null, view, react: true);
            var second = Render("src/page.tsx", "Development", true, null, view, react: true);
            StringAssert.Contains(first, "@react-refresh");
            StringAssert.Contains(first, "@vite/client");
            StringAssert.Contains(second, "http://localhost:5178/src/page.tsx");
            Assert.IsFalse(second.Contains("@react-refresh"));
            Assert.IsFalse(second.Contains("@vite/client"));
        }

        [DataTestMethod]
        [DataRow("Production", true)]
        [DataRow("Staging", true)]
        [DataRow("Development", false)]
        public void BuiltEntriesIncludeTransitiveCssAndNeverDevServer(string environment, bool enabled)
        {
            var root = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(Path.Combine(root, "modern", ".vite"));
            try
            {
                File.WriteAllText(Path.Combine(root, "modern", ".vite", "manifest.json"),
                    "{\"src/styles.css\":{\"file\":\"assets/styles-123.css\"},\"src/page.tsx\":{\"file\":\"assets/page-123.js\",\"css\":[\"assets/page-123.css\"],\"imports\":[\"shared\"]}," +
                    "\"shared\":{\"file\":\"assets/shared.js\",\"css\":[\"assets/shared.css\"],\"imports\":[\"nested\"]}," +
                    "\"nested\":{\"file\":\"assets/nested.js\",\"css\":[\"assets/nested.css\"]}}");
                var view = new ViewContext { HttpContext = new DefaultHttpContext() };
                view.HttpContext.Request.PathBase = "/purchasing";
                var css = Render("src/styles.css", environment, enabled, root, view);
                StringAssert.Contains(css, "href=\"/purchasing/modern/assets/styles-123.css\"");
                StringAssert.Contains(css, "rel=\"stylesheet\"");
                Assert.IsFalse(css.Contains("<script"));
                Assert.AreEqual("", Render("src/styles.css", environment, enabled, root, view));
                var html = Render("src/page.tsx", environment, enabled, root, view);
                StringAssert.Contains(html, "/purchasing/modern/assets/page-123.js");
                StringAssert.Contains(html, "/purchasing/modern/assets/page-123.css");
                StringAssert.Contains(html, "/purchasing/modern/assets/shared.css");
                StringAssert.Contains(html, "/purchasing/modern/assets/nested.css");
                Assert.IsFalse(html.Contains("localhost"));
                Assert.IsFalse(html.Contains("@vite"));
                Assert.AreEqual("", Render("src/page.tsx", environment, enabled, root, view));
            }
            finally { Directory.Delete(root, recursive: true); }
        }

        private static string Render(string entry, string environment, bool enabled, string root, ViewContext view, bool react = false)
        {
            var host = new Mock<IWebHostEnvironment>();
            host.SetupGet(x => x.EnvironmentName).Returns(environment);
            host.SetupGet(x => x.WebRootPath).Returns(root);
            var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
            {
                ["Vite:UseDevServer"] = enabled.ToString(),
                ["Vite:DevServerUrl"] = "http://localhost:5178"
            }).Build();
            var helper = new ViteEntryTagHelper(host.Object, config) { Src = entry, ViewContext = view, React = react };
            var context = new TagHelperContext(new TagHelperAttributeList(), new Dictionary<object, object>(), "test");
            var output = new TagHelperOutput("vite-entry", new TagHelperAttributeList(),
                (_, _) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));
            helper.Process(context, output);
            using var writer = new StringWriter();
            output.WriteTo(writer, HtmlEncoder.Default);
            return writer.ToString();
        }
    }
}
