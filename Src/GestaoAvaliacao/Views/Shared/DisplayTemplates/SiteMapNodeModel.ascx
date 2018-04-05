<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl`1[[MvcSiteMapProvider.Web.Html.Models.SiteMapNodeModel,MvcSiteMapProvider]]" %>
<%@ Import Namespace="System.Web.Mvc.Html" %>
<%@ Import Namespace="MvcSiteMapProvider.Web.Html.Models" %>

<% if (Model.IsCurrentNode && Model.SourceMetadata["HtmlHelper"].ToString() != "MvcSiteMapProvider.Web.Html.MenuHelper")  {
       if (Model.Title == "Home" && Model.Url == "/")
       {%>
            <%=Model.Title %>
<% }else{%>
           <%=Model.Title %>
<%  }
    }
   else if (Model.IsClickable && Model.Url.Contains("/"))
   {
       if (Model.Title == "Home" && Model.Url == "/")
       {%>
    
    <a href="<%=Model.Url %>"><%=Model.Title %></a>
<% } else { %>
    <a href="<%=Model.Url %>">><%=Model.Title %></a>
    <% }
   } else { %>
    <%=Model.Title %>
<% } %>