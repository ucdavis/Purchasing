using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using CommonServiceLocator;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Moq;
using UCDArch.Core;

namespace UCDArch.Testing.Fakes
{
    #region mocks
    /// <summary>
    /// Mock the HTTPContext. Used for getting the current user name
    /// Example Call: Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "John");
    /// </summary>
    public class MockHttpContext : HttpContext
    {
        private ClaimsPrincipal _user;
        private readonly int _count;
        public string[] UserRoles { get; set; }
        private string _userName;
        private string _fileContentType;
        private HttpRequest _httpRequest;
        private IServiceProvider _serviceProvider;

        public MockHttpContext(int fileCount, string[] userRoles, string userName = "UserName", string fileContentType = "application/pdf")
        {
            _count = fileCount;
            UserRoles = userRoles;
            _userName = userName;
            _fileContentType = fileContentType;
            _httpRequest = Mock.Of<HttpRequest>();
            _serviceProvider = Mock.Of<IServiceProvider>();
            Mock.Get(_serviceProvider).Setup(x => x.GetService(It.IsAny<Type>())).Returns<Type>(a => ServiceLocator.Current.GetService(a));
        }

        public override ClaimsPrincipal User
        {
            get { return _user ?? (_user = new MockPrincipal(UserRoles, _userName)); }
            set
            {
                _user = value;
            }
        }

        public override HttpRequest Request
        {
            get
            {
                return _httpRequest;
            }
        }

        public override IFeatureCollection Features => throw new NotImplementedException();

        public override HttpResponse Response => throw new NotImplementedException();

        public override ConnectionInfo Connection => throw new NotImplementedException();

        public override WebSocketManager WebSockets => throw new NotImplementedException();

        public override IDictionary<object, object> Items { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override IServiceProvider RequestServices { get => _serviceProvider; set => throw new NotImplementedException(); }
        public override CancellationToken RequestAborted { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override string TraceIdentifier { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override ISession Session { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override void Abort()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Mock the Principal. Used for getting the current user name
    /// </summary>
    public class MockPrincipal : ClaimsPrincipal
    {
        IIdentity _identity;
        public bool RoleReturnValue { get; set; }
        public string[] UserRoles { get; set; }
        private string _userName;

        public MockPrincipal(string[] userRoles, string userName = "UserName")
        {
            UserRoles = userRoles;
            _userName = userName;
        }

        public override IIdentity Identity
        {
            get { return _identity ?? (_identity = new MockIdentity(_userName)); }
        }

        public override bool IsInRole(string role)
        {
            if (UserRoles.Contains(role))
            {
                return true;
            }
            return false;
        }

    }

    /// <summary>
    /// Mock the Identity. Used for getting the current user name
    /// </summary>
    public class MockIdentity : IIdentity
    {
        private string _userName;


        public MockIdentity(string userName = "UserName")
        {
            _userName = userName;
        }

        public string AuthenticationType
        {
            get
            {
                return "MockAuthentication";
            }
        }

        public bool IsAuthenticated
        {
            get
            {
                return true;
            }
        }

        public string Name
        {
            get
            {
                return _userName;
            }
        }
    }





    // public class MockHttpRequest : HttpRequest
    // {
    //     //MockHttpFileCollectionBase Mocked { get; set; }

    //     public MockHttpRequest(int count, string contentType = "application/pdf")
    //     {
    //         //Mocked = new MockHttpFileCollectionBase(count, contentType);
    //     }
    //     public override HttpFileCollectionBase Files
    //     {
    //         get
    //         {
    //             return Mocked;
    //         }
    //     }
    // }

    // public class MockHttpFileCollectionBase : HttpFileCollectionBase
    // {
    //     public int Counter { get; set; }
    //     private string _contentType;

    //     public MockHttpFileCollectionBase(int count, string contentType = "application/pdf")
    //     {
    //         _contentType = contentType;
    //         Counter = count;
    //         for (int i = 0; i < count; i++)
    //         {
    //             BaseAdd("Test" + (i + 1), new byte[] { 4, 5, 6, 7, 8 });
    //         }

    //     }

    //     public override int Count
    //     {
    //         get
    //         {
    //             return Counter;
    //         }
    //     }
    //     public override HttpPostedFileBase Get(string name)
    //     {
    //         return new MockHttpPostedFileBase(_contentType);
    //     }
    //     public override HttpPostedFileBase this[string name]
    //     {
    //         get
    //         {
    //             return new MockHttpPostedFileBase(_contentType);
    //         }
    //     }
    //     public override HttpPostedFileBase this[int index]
    //     {
    //         get
    //         {
    //             return new MockHttpPostedFileBase(_contentType);
    //         }
    //     }
    // }

    // public class MockHttpPostedFileBase : HttpPostedFileBase
    // {
    //     private string _contentType;
    //     public MockHttpPostedFileBase(string contentType)
    //     {
    //         _contentType = contentType;
    //     }
    //     public override int ContentLength
    //     {
    //         get
    //         {
    //             return 5;
    //         }
    //     }
    //     public override string FileName
    //     {
    //         get
    //         {
    //             return "Mocked File Name";
    //         }
    //     }
    //     public override Stream InputStream
    //     {
    //         get
    //         {
    //             var memStream = new MemoryStream(new byte[] { 4, 5, 6, 7, 8 });
    //             return memStream;
    //         }
    //     }
    //     public override string ContentType
    //     {
    //         get
    //         {
    //             return _contentType;
    //         }
    //     }
    // }

    #endregion
}
