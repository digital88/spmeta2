﻿using System.Net;
using Microsoft.SharePoint.Client;
using SPMeta2.Containers.Assertion;
using SPMeta2.CSOM.ModelHandlers;
using SPMeta2.CSOM.ModelHosts;
using SPMeta2.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SPMeta2.Definitions.Base;
using SPMeta2.Enumerations;
using SPMeta2.Utils;

namespace SPMeta2.Regression.CSOM.Validation
{
    public class ClientWebPartDefinitionValidator : WebPartModelHandler
    {
        public override void DeployModel(object modelHost, DefinitionBase model)
        {
            var listItemModelHost = modelHost.WithAssertAndCast<ListItemModelHost>("modelHost", value => value.RequireNotNull());
            var definition = model.WithAssertAndCast<WebPartDefinition>("model", value => value.RequireNotNull());

            var pageItem = listItemModelHost.HostListItem;

            var context = pageItem.Context;

            var siteServerUrl = listItemModelHost.HostSite.ServerRelativeUrl;
            var webUrl = listItemModelHost.HostWeb.Url;

            var serverUrl = context.Url.Split(new string[] { siteServerUrl }, StringSplitOptions.RemoveEmptyEntries)[0];
            var absItemUrl = UrlUtility.CombineUrl(serverUrl, pageItem["FileRef"].ToString());

            WithWithExistingWebPart(pageItem, definition, (spObject, spObjectDefintion) =>
            {
                var webpartExportUrl = UrlUtility.CombineUrl(new[]{
                        webUrl,
                        "_vti_bin/exportwp.aspx?pageurl=" + absItemUrl + "&" + "guidstring=" + spObjectDefintion.Id.ToString()
                    });

                var assert = ServiceFactory.AssertService
                                           .NewAssert(model, definition, spObject)
                                                 .ShouldNotBeNull(spObject);

                var webClient = new WebClient();

                if (context.Credentials != null)
                    webClient.Credentials = context.Credentials;
                else
                    webClient.UseDefaultCredentials = true;

                var webPartXmlString = webClient.DownloadString(webpartExportUrl);
                var webPartXml = WebpartXmlExtensions.LoadWebpartXmlDocument(webPartXmlString);


                assert.ShouldBeEqual(m => m.Title, o => o.Title);

                //if (!string.IsNullOrEmpty(definition.ChromeState))
                //{
                //    var value = webPartXml.GetProperty("ChromeState");

                //    assert.ShouldBeEqual((p, s, d) =>
                //    {
                //        var srcProp = s.GetExpressionValue(m => m.ChromeState);
                //        var isValid = definition.ChromeState == value;

                //        return new PropertyValidationResult
                //        {
                //            Tag = p.Tag,
                //            Src = srcProp,
                //            Dst = null,
                //            IsValid = isValid
                //        };
                //    });
                //}
                //else
                assert.SkipProperty(m => m.ChromeState, "ChromeState is null or empty. Skipping.");

                //if (!string.IsNullOrEmpty(definition.ChromeType))
                //{
                //    var value = webPartXml.GetProperty("ChromeType");

                //    assert.ShouldBeEqual((p, s, d) =>
                //    {
                //        var srcProp = s.GetExpressionValue(m => m.ChromeType);
                //        var isValid = definition.ChromeType == value;

                //        return new PropertyValidationResult
                //        {
                //            Tag = p.Tag,
                //            Src = srcProp,
                //            Dst = null,
                //            IsValid = isValid
                //        };
                //    });
                //}
                //else
                assert.SkipProperty(m => m.ChromeType, "ChromeType is null or empty. Skipping.");

                if (!string.IsNullOrEmpty(definition.Description))
                {
                    // todo
                }
                else
                    assert.SkipProperty(m => m.Description, "Description is null or empty. Skipping.");

                if (definition.Height.HasValue)
                {
                    // todo
                }
                else
                    assert.SkipProperty(m => m.Height, "Height is null or empty. Skipping.");

                if (definition.Width.HasValue)
                {
                    var value = ConvertUtils.ToInt(webPartXml.GetProperty("Width").Replace("px", string.Empty));

                    assert.ShouldBeEqual((p, s, d) =>
                    {
                        var srcProp = s.GetExpressionValue(m => m.Width);
                        var isValid = definition.Width == value;

                        return new PropertyValidationResult
                        {
                            Tag = p.Tag,
                            Src = srcProp,
                            Dst = null,
                            IsValid = isValid
                        };
                    });
                }
                else
                {
                    assert.SkipProperty(m => m.Width, "Width is NULL, skipping");
                }

                if (!string.IsNullOrEmpty(definition.ImportErrorMessage))
                {
                    // todo
                }
                else
                    assert.SkipProperty(m => m.ImportErrorMessage, "ImportErrorMessage is null or empty. Skipping.");

                if (!string.IsNullOrEmpty(definition.TitleIconImageUrl))
                {
                    // todo
                }
                else
                    assert.SkipProperty(m => m.TitleIconImageUrl, "TitleIconImageUrl is null or empty. Skipping.");

                if (!string.IsNullOrEmpty(definition.TitleUrl))
                {
                    // todo
                }
                else
                    assert.SkipProperty(m => m.TitleUrl, "TitleUrl is null or empty. Skipping.");


                assert.SkipProperty(m => m.WebpartFileName, "WebpartFileName is null or empty. Skipping.");
                assert.SkipProperty(m => m.WebpartType, "WebpartType is null or empty. Skipping.");
                assert.SkipProperty(m => m.WebpartXmlTemplate, "WebpartXmlTemplate is null or empty. Skipping.");
                assert.SkipProperty(m => m.ZoneId, "ZoneId is null or empty. Skipping.");
                assert.SkipProperty(m => m.ZoneIndex, "ZoneIndex is null or empty. Skipping.");
                assert.SkipProperty(m => m.Id, "Id is null or empty. Skipping.");
            });
        }
    }
}
