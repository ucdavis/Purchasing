using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Core.Domain;
using Purchasing.Mvc.Models;
using Purchasing.Tests.Support;

namespace Purchasing.Tests.ControllerTests
{
    [TestClass]
    [DoNotParallelize]
    public class OrderReviewWebTests
    {
        private AwayStatusWebHost _site;
        private HttpClient _client;

        [TestCleanup]
        public void StopHost()
        {
            _client?.Dispose();
            _site?.Dispose();
        }

        private async Task StartHost(ReviewOrderViewModel model, bool modern = true)
        {
            _site = new AwayStatusWebHost();
            await _site.StartAsync(useDevServer: true, reviewModel: model);
            _site.Configuration["ModernUi:Enabled"] = modern.ToString();
            _site.Configuration["ModernUi:AllowedLogins:0"] = "away-test";
            _client = _site.Host.GetTestClient();
        }

        private async Task<string> GetReview()
        {
            using var response = await _client.GetAsync("/Order/Review/42");
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "The real review Razor view should render successfully.");
            return await response.Content.ReadAsStringAsync();
        }

        private static void Includes(string html, string expected)
        {
            // Do not include rendered HTML in assertion messages: it contains antiforgery tokens.
            Assert.IsTrue(html.Contains(expected), $"Rendered review should include {expected}.");
        }

        private static void Excludes(string html, string unexpected)
        {
            Assert.IsFalse(html.Contains(unexpected), $"Rendered review should not include {unexpected}.");
        }

        private static string PostForm(string html, string action, bool routeId = false)
        {
            var expectedAction = routeId ? $"/Order/{action}/42" : $"/Order/{action}(?:/42)?";
            var form = Regex.Matches(html, @"<form\b[^>]*>.*?</form>", RegexOptions.Singleline)
                .Cast<Match>().Select(match => match.Value)
                .SingleOrDefault(value => Regex.IsMatch(value, $"action=\"{expectedAction}\""));
            Assert.IsNotNull(form, $"The {action} form should post to the existing Order endpoint.");
            Assert.IsTrue(Regex.IsMatch(form, "method=\"post\"", RegexOptions.IgnoreCase));
            Assert.IsTrue(Regex.IsMatch(form, "name=\"__RequestVerificationToken\"[^>]*value=\"[^\"]+\""),
                $"The {action} form must contain an antiforgery token.");
            return form;
        }

        private static void AssertInterface(string html, bool modern)
        {
            if (modern)
            {
                Includes(html, "class=\"modern-order-review space-y-6\"");
                Includes(html, "http://localhost:5178/src/styles.css");
                Includes(html, "SkipNavigation: true");
                Excludes(html, "Scripts/public/common/startup.js");
                Excludes(html, "Scripts/public/single/SausageCustom.js");
                Excludes(html, "Scripts/public/single/jquery.stickyfloat.js");
            }
            else
            {
                Excludes(html, "modern-order-review");
                Excludes(html, "http://localhost:5178");
                Excludes(html, "SkipNavigation: true");
                Includes(html, "Scripts/public/common/startup.js");
                Includes(html, "Scripts/public/single/SausageCustom.js");
            }
            Includes(html, "Scripts/external/jquery/jquery-1.7.1.js");
            Includes(html, "Scripts/external/jquery-ui/jquery-ui-1.8.17.js");
            Includes(html, "Scripts/public/single/OrderReview.js");
            Includes(html, "Scripts/public/common/jquery.qtip.js");
            Includes(html, "Scripts/public/common/jquery.validate.js");
            Includes(html, "Scripts/public/common/jquery.validate.unobtrusive.js");
            Includes(html, "Scripts/public/single/fileuploader.js");
            Includes(html, "Scripts/public/single/jquery.tmpl.min.js");
        }

        [DataTestMethod]
        [DataRow(null, null, false)]
        [DataRow("false", "away-test", false)]
        [DataRow("true", null, false)]
        [DataRow("true", "someone-else", false)]
        [DataRow("true", "AWAY-TEST", true)]
        public async Task ReviewRequiresBothGlobalEnableAndMatchingLogin(string enabled, string login, bool modern)
        {
            await StartHost(CreateModel());
            _site.Configuration["ModernUi:Enabled"] = enabled;
            _site.Configuration["ModernUi:AllowedLogins:0"] = login;
            AssertInterface(await GetReview(), modern);
        }

        [DataTestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task PurchaserReviewRetainsSharedSectionsActionsAndWidgetDependencies(bool modern)
        {
            await StartHost(CreateModel(), modern);
            var html = await GetReview();
            AssertInterface(html, modern);
            foreach (var id in new[] { "order-details", "line-items", "accounts", "notes", "attachments", "approvals", "history", "favs-dialog", "file-uploader", "peepsDialog" })
                Includes(html, $"id=\"{id}\"");
            Includes(html, "Test service announcement");
            Includes(html, "Synthetic laboratory supplies");
            Includes(html, "Synthetic review note");
            Includes(html, "Synthetic order history");
            Includes(html, "/Order/Edit/42");
            Includes(html, "/Order/Copy/42");
            Includes(html, "/Order/ReroutePurchaser/42");
            Includes(html, "/Report/Invoice/42");
            Assert.IsTrue(Regex.IsMatch(html, "showOrderHistory=true", RegexOptions.IgnoreCase));
            Assert.IsTrue(Regex.IsMatch(html, "forVendor=true", RegexOptions.IgnoreCase));

            var decision = PostForm(html, "Approve", routeId: true);
            Includes(decision, "name=\"comment\"");
            Includes(decision, "name=\"orderType\"");
            Includes(decision, "id=\"complete-order\"");
            Includes(decision, "id=\"deny-order\"");
            var cancel = PostForm(html, "Cancel");
            Includes(cancel, "name=\"id\"");
            Includes(cancel, "name=\"comment\"");
            Includes(cancel, "required");
            Excludes(html, "id=\"cancel-completed-order-form\"");
        }

        [DataTestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task CompletedReviewRendersWithoutPendingApprovalCollectionsAndRetainsFollowupActions(bool modern)
        {
            var model = CreateModel(complete: true);
            Assert.IsNull(model.OrderedUniqueApprovals);
            Assert.IsNull(model.OrderTypes);
            await StartHost(model, modern);
            var html = await GetReview();
            AssertInterface(html, modern);
            Excludes(html, "id=\"approvals\"");
            Excludes(html, "id=\"complete-order\"");
            Excludes(html, "id=\"cancel-form\"");
            Excludes(html, "/Order/Edit/42");
            Includes(html, "/Order/ReceiveItems/42");
            Includes(html, "/Order/PayInvoice/42");
            Includes(html, "/Order/AssignAccountsPayableUser/42");
            Includes(html, "id=\"edit-po-number\"");
            Includes(html, "id=\"edit-reference-number\"");
            Includes(html, "id=\"edit-ordernotes\"");
            Includes(html, "class=\"PostStatusDDL\"");
            var cancel = PostForm(html, "CancelCompletedOrder");
            Includes(cancel, "name=\"comment\"");
            Includes(cancel, "required");
            var fpd = PostForm(html, "FpdComplete");
            Includes(fpd, "name=\"fpdCompleted\"");
        }

        [TestMethod]
        public async Task InvalidAccountsBlockCompletionWhileKeepingDenialAndWarnings()
        {
            var model = CreateModel();
            model.HasInvalidAccounts = true;
            model.InvalidAccountsMessage = "Synthetic invalid account";
            model.AccountsWarningsMessage = "Synthetic account warning";
            await StartHost(model);
            var html = await GetReview();
            Includes(html, "Synthetic invalid account");
            Includes(html, "Synthetic account warning");
            Includes(PostForm(html, "Approve", routeId: true), "id=\"deny-order\"");
            Excludes(html, "id=\"complete-order\"");
        }

        [TestMethod]
        public async Task ReadonlyReviewKeepsNotesAttachmentsAndFavoritesWithoutEditOrDecisionActions()
        {
            var model = CreateModel();
            model.CanEditOrder = false;
            model.CanCancelOrder = false;
            model.IsRequesterInWorkgroup = false;
            await StartHost(model);
            var html = await GetReview();
            Includes(html, "id=\"add-note\"");
            Includes(html, "id=\"file-uploader\"");
            Includes(html, "id=\"toggle-fav\"");
            Excludes(html, "/Order/Edit/42");
            Excludes(html, "/Order/Copy/42");
            Excludes(html, "id=\"complete-order\"");
            Excludes(html, "id=\"deny-order\"");
            Excludes(html, "id=\"cancel-form\"");
        }

        private static ReviewOrderViewModel CreateModel(bool complete = false)
        {
            var user = new User("away-test") { FirstName = "Review", LastName = "Tester", Email = "review@example.test" };
            var status = new OrderStatusCode
            {
                Id = complete ? OrderStatusCode.Codes.Complete : OrderStatusCode.Codes.Purchaser,
                Name = complete ? "Complete" : "Purchaser", IsComplete = complete, Level = complete ? 5 : 4
            };
            var order = new Order
            {
                Id = 42, StatusCode = status,
                OrderType = new OrderType(complete ? OrderType.Types.PurchasingCard : OrderType.Types.OrderRequest) { Name = complete ? "Purchasing Card" : "Order Request" },
                Workgroup = new Workgroup { Id = 7, Name = "Synthetic Workgroup" },
                Organization = new Organization { Id = "TEST", Name = "Synthetic Department" },
                ShippingType = new ShippingType(ShippingType.Codes.Standard) { Name = "Standard" },
                Address = new WorkgroupAddress { Address = "1 Test Way", City = "Davis", State = "CA", Zip = "95616", AeLocationCode = "TEST" },
                CreatedBy = user, DateCreated = new DateTime(2030, 1, 15), DateNeeded = new DateTime(2030, 2, 1),
                DeliverTo = user.FullName, DeliverToEmail = user.Email, Justification = "Synthetic purchase justification",
                ReferenceNumber = complete ? "CARD-42" : null, PoNumber = complete ? "PO-42" : null,
                TotalFromDb = 100m, EstimatedTax = 0m
            };
            order.GenerateRequestNumber();
            order.LineItems.Add(new LineItem { Id = 1, Order = order, Quantity = 2m, Unit = "EA", UnitPrice = 50m, Description = "Synthetic laboratory supplies" });
            order.Splits.Add(new Split { Id = 1, Order = order, Account = "TEST001", Amount = 100m });
            order.OrderComments.Add(new OrderComment { Order = order, User = user, DateCreated = order.DateCreated, Text = "Synthetic review note" });
            order.OrderTrackings.Add(new OrderTracking { Order = order, User = user, StatusCode = status, DateCreated = order.DateCreated, Description = "Synthetic order history" });
            if (!complete)
                order.Approvals.Add(new Approval { Id = 1, Order = order, User = user, StatusCode = status });

            return new ReviewOrderViewModel
            {
                Order = order, Favorite = new Favorite { Order = order, User = user, IsActive = false },
                Complete = complete, Status = status.Name, CurrentUser = user.Id,
                WorkgroupName = order.Workgroup.Name, OrganizationName = order.Organization.Name,
                Address = order.Address, LineItems = order.LineItems, Splits = order.Splits,
                Comments = order.OrderComments, Attachments = order.Attachments,
                CustomFieldsAnswers = order.CustomFieldAnswers, OrderTracking = order.OrderTrackings,
                Approvals = order.Approvals,
                OrderedUniqueApprovals = complete ? null : order.Approvals.OrderBy(approval => approval.StatusCode.Level),
                ApprovalUsers = complete ? null : new[] { user },
                CanEditOrder = !complete, CanCancelOrder = !complete, CanCancelCompletedOrder = complete,
                IsPurchaser = !complete, IsRequesterInWorkgroup = true,
                UserRoles = new HashSet<string> { Role.Codes.Purchaser },
                OrderTypes = complete ? null : new List<OrderType>
                {
                    new(OrderType.Types.AggieEnterprise) { Name = "Aggie Enterprise", PurchaserAssignable = true },
                    new(OrderType.Types.PurchasingCard) { Name = "Purchasing Card", PurchaserAssignable = true }
                }
            };
        }
    }
}
