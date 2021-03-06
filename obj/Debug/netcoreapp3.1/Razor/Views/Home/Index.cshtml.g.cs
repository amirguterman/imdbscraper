#pragma checksum "C:\Users\amirg_000\ImdbListingExercise\Views\Home\Index.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "15c37ea1d47363e2c86d00026cb667fe2334c27a"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Home_Index), @"mvc.1.0.view", @"/Views/Home/Index.cshtml")]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#nullable restore
#line 1 "C:\Users\amirg_000\ImdbListingExercise\Views\_ViewImports.cshtml"
using ImdbListingExercise;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "C:\Users\amirg_000\ImdbListingExercise\Views\_ViewImports.cshtml"
using ImdbListingExercise.Models;

#line default
#line hidden
#nullable disable
#nullable restore
#line 3 "C:\Users\amirg_000\ImdbListingExercise\Views\_ViewImports.cshtml"
using ImdbListingExercise.Models.Imdb;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"15c37ea1d47363e2c86d00026cb667fe2334c27a", @"/Views/Home/Index.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"98d8d7abc6b5f396b6eb49aaa15e7624930be180", @"/Views/_ViewImports.cshtml")]
    public class Views_Home_Index : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 1 "C:\Users\amirg_000\ImdbListingExercise\Views\Home\Index.cshtml"
  
    ViewData["Title"] = "Actors List";

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n<h1>IMDB Scraper - Actors</h1>\r\n\r\n<p id=\"section-status\">status: <span id=\"status\">");
#nullable restore
#line 7 "C:\Users\amirg_000\ImdbListingExercise\Views\Home\Index.cshtml"
                                            Write(ViewData["SyncStatus"]);

#line default
#line hidden
#nullable disable
            WriteLiteral(" (");
#nullable restore
#line 7 "C:\Users\amirg_000\ImdbListingExercise\Views\Home\Index.cshtml"
                                                                     Write(ViewData["ActorsCount"]);

#line default
#line hidden
#nullable disable
            WriteLiteral(" actors)</span></p><br/>\r\n\r\n<p id=\"section-progress\" hidden=\"true\">progress: <span id=\"progress\">");
#nullable restore
#line 9 "C:\Users\amirg_000\ImdbListingExercise\Views\Home\Index.cshtml"
                                                                Write(ViewData["SyncProgress"]);

#line default
#line hidden
#nullable disable
            WriteLiteral("%</span></p><br/>\r\n\r\n<p id=\"section-task\" hidden=\"true\">current task: <span id=\"task\">");
#nullable restore
#line 11 "C:\Users\amirg_000\ImdbListingExercise\Views\Home\Index.cshtml"
                                                            Write(ViewData["CurrentTask"]);

#line default
#line hidden
#nullable disable
            WriteLiteral(@"</span></p><br/>

<p id=""section-StartSync"">
    <span>This will add only the missing actors in local db (will not update info of existing actors):</span><br/>
    <input type=""button"" id=""btnStartSync"" value=""Start Sync""><br/><br/>
</p>

<p id=""section-StopSync"" hidden=""true"">
    <span>This will stop the sync job and keep the actors that were already synced:</span><br/>
    <input type=""button"" id=""btnStopSync"" value=""Stop Sync""><br/><br/>
</p>

<p id=""section-Reset"">
    <span>This will empty the local db and reload all the actors:</span><br/>
    <input type=""button"" id=""btnReset"" value=""Reset""><br/><br/>
</p>

<table id=""dvActors"">
    <caption>My Actors List</caption>
    <tr class=""header"">
        <th scope=""col"">Name</th>
        <th scope=""col"">Gender</th>
        <th scope=""col"">Role</th>
        <th scope=""col"">Birth Date</th>
        <th scope=""col"">Photo</th>
        <th scope=""col""></th>
    </tr>
</table>

");
            DefineSection("Scripts", async() => {
                WriteLiteral("\r\n");
#nullable restore
#line 41 "C:\Users\amirg_000\ImdbListingExercise\Views\Home\Index.cshtml"
      await Html.RenderPartialAsync("_ValidationScriptsPartial");

#line default
#line hidden
#nullable disable
                WriteLiteral("    <script type=\"text/javascript\" language=\"JavaScript\">\r\n        var syncInProgress = ");
#nullable restore
#line 43 "C:\Users\amirg_000\ImdbListingExercise\Views\Home\Index.cshtml"
                        Write(ViewData["IsSyncing"]);

#line default
#line hidden
#nullable disable
                WriteLiteral(@";
        $(document).ready(function () {
            refreshList();
            window.setInterval(getStatus, 1000);

            function getStatus() {
                $.ajax({
                    type: ""GET"",
                    url: ""/Actors/GetStatus"",
                    contentType: ""application/json"",
                    dataType: ""json"",
                    success: function (response) {
                        $(""#status"").text(response.status + ' (' + response.actorsCount + ' actors)');

                        if (response.status == 'idle') {
                            syncInProgress = false;
                            $(""#progress"").text("""");
                            $(""#section-progress"").attr('hidden', true);
                            
                            $(""#task"").text("""");
                            $(""#section-task"").attr('hidden', true);
                            
                            $('#section-StartSync').removeAttr('hidden');
            ");
                WriteLiteral(@"                $('#section-StopSync').attr('hidden', true);
                
                        }
                        else if (response.status == 'sync') {
                            syncInProgress = true;
                            $(""#progress"").text(response.progress + '%');
                            $(""#section-progress"").removeAttr('hidden');
                            
                            $(""#task"").text(response.currentTask);
                            $(""#section-task"").removeAttr('hidden');

                            $('#section-StartSync').attr('hidden', true);
                            $('#section-StopSync').removeAttr('hidden');

                            refreshList();
                        }
                    },
                    failure: function (response) {
                        alert(response);
                    }
                });  
            }
            function deleteActor(id) {
                
                $.ajax({");
                WriteLiteral(@"
                    type: ""DELETE"",
                    url: ""/Actors/Delete/"" + id,
                    contentType: ""application/json"",
                    dataType: ""json"",
                    success: function (response) {
                        
                        var dvActors = $(""#dvActors"");
                        var actor = $(""#actor-"" + id);
                        actor.remove();
                    },
                    failure: function (response) {
                        alert(response);
                    }
                });
            
            }

            function startSync() {
                //syncInProgress = true;
                $.ajax({
                    type: ""POST"",
                    url: ""/Actors/StartSync"",
                    contentType: ""application/json; charset=utf-8"",
                    dataType: ""json"",
                    success: function (response) {

                    },
                    failure: function (respons");
                WriteLiteral(@"e) {
                        alert(response);
                    }
                });
            }

            function stopSync() {
                $.ajax({
                    type: ""POST"",
                    url: ""/Actors/StopSync"",
                    contentType: ""application/json; charset=utf-8"",
                    dataType: ""json"",
                    success: function (response) {

                    },
                    failure: function (response) {
                        alert(response);
                    }
                });
            }

            function refreshList() {
                $.ajax({
                    type: ""GET"",
                    url: ""/Actors/GetList"",
                    contentType: ""application/json"",
                    dataType: ""json"",
                    success: function (response) {
                        var dvActors = $(""#dvActors"");
                        $('.actor-row').remove();

                        $.each(resp");
                WriteLiteral(@"onse, function (i, actor) {
                            var aDelete = $('<input>')
                                .attr('type', 'button')
                                .attr('id', 'btnDelete-' + actor.id)
                                .attr('value', 'Delete');

                            $(aDelete).on('click', function () { 
                                deleteActor(actor.id); 
                            });

                            var $tr = $('<tr>')
                                .attr('id', 'actor-' + actor.id)
                                .attr('class', 'actor-row')
                                .append($('<td>').append(aDelete))
                                .append($('<th>').attr('scope', 'row').text(actor.fullName))
                                .append($('<td>').text(actor.gender))
                                .append($('<td>').text(actor.role))
                                .append($('<td>').text(actor.born))
                                .append($('<t");
                WriteLiteral(@"d>').append($('<img>').attr('src', actor.imageUrl)))
                                .appendTo(dvActors);
                        });
                    },
                    failure: function (response) {
                        alert(response);
                    }
                });
            }

            $('#btnReset').on('click', function () {
                //syncInProgress = true;
                $.ajax({
                    type: ""POST"",
                    url: ""/Actors/Reset"",
                    contentType: ""application/json; charset=utf-8"",
                    dataType: ""json"",
                    success: function (response) {
                        /*syncInProgress = false;
                        var dvActors = $(""#dvActors"");
                        dvActors.empty();
                        $.each(response, function (i, item) {
                            var $tr = $('<li>').append(item).appendTo(dvActors);
                        });*/
                    },");
                WriteLiteral(@"
                    failure: function (response) {
                        alert(response);
                    }
                });
            });

            $('#btnStartSync').on('click', startSync);
            $('#btnStopSync').on('click', stopSync);

        });
    </script>
");
            }
            );
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<dynamic> Html { get; private set; }
    }
}
#pragma warning restore 1591
