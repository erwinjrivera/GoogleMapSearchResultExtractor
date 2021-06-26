using GoogleMapSearchResultExtractor.Model;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace GoogleMapSearchResultExtractor.Service
{
    public class GoogleSearchService : IGoogleService
    {

        public List<MapResult> GetListOfMapResult(string keyword, int offset)
        {
            int start = 0;
            int count = 0;
            List<MapResult> list = new List<MapResult>();

            string queryString = $"https://www.google.com/search?q={HttpUtility.UrlEncode(keyword)}&ie=UTF-8&start={offset}&sa=N&rlst=f&cr=countryUS";
            var htmlDocument = GetHtmlDocument(queryString);

            if (htmlDocument != null)
            {
                var nodes = htmlDocument.DocumentNode.SelectNodes("//div[@class='X7NTVe']");
                //var node = htmlDocument.DocumentNode.SelectSingleNode("//div[@class='X7NTVe']");

                if (nodes != null)
                {
                    Debug.WriteLine("");
                    Debug.WriteLine($"Query: {queryString}");
                    Debug.WriteLine("");

                    foreach (var node in nodes)
                    {
                        var url = GetAttributeValue("href", node, ".//a[@class='tHmfQe']");
                        var title = GetInnerText(node, ".//div[@class='BNeawe deIvCb AP7Wnd']");

                        var item = new MapResult();

                        item.Title = title;
                        item.URL = url;

                        Debug.WriteLine("");
                        Debug.WriteLine($"Title: {title}");
                        Debug.WriteLine($"URL: {url}");

                        list.Add(item);

                        //break;

                    } //for
                }
            }


            return list;
        }

        public MapResult GetCardDetails(string url, string title = "")
        {
            var item = new MapResult();

            //foreach (var item in list)
            {
                HtmlDocument cardHtmlDocument = GetHtmlDocument(url);

                if (cardHtmlDocument != null)
                {
                    Debug.WriteLine("");
                    Debug.WriteLine($"------>Processing card details:");
                    Debug.WriteLine($"----------->Title: {title}");
                    Debug.WriteLine($"----------->URL: {url}");

                    try
                    {
                        var cardNode = cardHtmlDocument.DocumentNode.SelectSingleNode("//div[@class='ZINbbc xpd O9g5cc uUPGi']"); // organic

                        string rating = null;
                        string reviews = null;
                        string description = null;
                        string website = null;
                        string address = null;
                        string hours = null;
                        string phone = null;

                        if (cardNode != null)
                        {
                            rating = GetInnerText(cardNode, "//span[@class='Eq0J8 oqSTJd']");
                            reviews = GetInnerText(cardNode, "//span[@class='Eq0J8']");
                            description = GetInnerText(cardNode, "//div[@class='BNeawe tAd8D AP7Wnd']");
                            website = ExtractWebsite(cardNode);
                            address = ExtractAddressAndContactInfo(cardNode, "Address");
                            hours = ExtractAddressAndContactInfo(cardNode, "Hours");
                            phone = ExtractAddressAndContactInfo(cardNode, "Phone");
                            description = CleanDescription(description, rating, reviews);
                            reviews = CleanReviews(reviews);

                            item.Title = title;
                            item.Description = description;
                            item.URL = url;
                            item.Rating = rating;
                            item.Reviews = reviews;
                            item.Website = website;

                            if (string.IsNullOrEmpty(website))
                            {
             
                                item.Website = GetWebSiteBasedFromTitle(title);
                            }

                            item.Address = address;
                            item.Hours = hours;
                            item.Phone = phone;
                            item.Email = SearchEmails(item.Title, item.Website);

                            
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("An error has occured while extracting card details. " + ex.Message);
                    }
                } //if
            }

            return item;
        }

        public string GetWebSiteBasedFromTitle(string title)
        {
            string website = null;

            try
            {
                string queryString = $"https://www.google.com/search?q=website+of+{title}";
                var htmlDocument = GetHtmlDocument(queryString);

                if (htmlDocument != null)
                {
                    var node = htmlDocument.DocumentNode.SelectSingleNode("//div[@class='kCrYT']/a");

                    if (node != null)
                    {
                        var hrefValue = GetAttributeValue("href", node);

                        if (hrefValue != null)
                        {
                            website = ExtractDomainFromUrl(hrefValue);

                            if (!string.IsNullOrEmpty(website))
                            {
                                if (!website.Contains(@"https://"))
                                {
                                    website = @"https://" + website;
                                }
                            }
                               
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error occured while trying to get website from title. " + ex.Message);
            }

            
            return website;
        }

        public List<MapResult> GoogleMapSearch(string keyword, int offset)
        {
            int start = 0;
            int count = 0;
            List<MapResult> list = new List<MapResult>();

            string queryString = $"https://www.google.com/search?q={HttpUtility.UrlEncode(keyword)}&ie=UTF-8&start={offset}&sa=N&rlst=f";
            var htmlDocument = GetHtmlDocument(queryString);

            if (htmlDocument != null)
            {
                //var nodes = htmlDocument.DocumentNode.SelectNodes("//div[@class='X7NTVe']");
                var node = htmlDocument.DocumentNode.SelectSingleNode("//div[@class='X7NTVe']");

                if (node != null)
                {
                    
                    //foreach (var node in nodes)
                    {
                        var url = GetAttributeValue("href", node, ".//a[@class='tHmfQe']");
                        var title = GetInnerText(node, ".//div[@class='BNeawe deIvCb AP7Wnd']");

                        var item = new MapResult();

                        item.Title = title;
                        item.URL = url;

                        Debug.WriteLine("");
                        Debug.WriteLine($"{offset + 1}.");
                        Debug.WriteLine($"Query: {queryString}");
                        Debug.WriteLine($"Title: {title}");
                        Debug.WriteLine($"URL: {url}");
               

                        HtmlDocument cardHtmlDocument = GetHtmlDocument(url);

                        if (cardHtmlDocument != null)
                        {
                            try
                            {
                                var cardNode = cardHtmlDocument.DocumentNode.SelectSingleNode(".//div[@class='ZINbbc xpd O9g5cc uUPGi']"); // organic

                                if (cardNode != null)
                                {
                                    var rating = GetInnerText(cardNode, ".//span[@class='Eq0J8 oqSTJd']");
                                    var reviews = GetInnerText(cardNode, ".//span[@class='Eq0J8']");
                                    var description = GetInnerText(cardNode, ".//div[@class='BNeawe tAd8D AP7Wnd']");
                                    var website = ExtractWebsite(cardNode);
                                    var address = ExtractAddressAndContactInfo(cardNode, "Address");
                                    var hours = ExtractAddressAndContactInfo(cardNode, "Hours");
                                    var phone = ExtractAddressAndContactInfo(cardNode, "Phone");



                                    description = CleanDescription(description, rating, reviews);
                                    reviews = CleanReviews(reviews);

                                    item.Description = description;
                                    item.URL = url;
                                    item.Rating = rating;
                                    item.Reviews = reviews;
                                    item.Website = website;
                                    item.Address = address;
                                    item.Hours = hours;
                                    item.Phone = phone;
                                    item.Email = SearchEmails(title);




                                   
                                }
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex.Message);
                            }
                        } //if

                        list.Add(item);

                        //break;

                    } //for
                }
                



            }


            return list;
        }

        private string GetEmailsFromWebsitePage(string url)
        {
            string htmlString = GetHtmlString(url);
            string emails = null;

            if (!String.IsNullOrEmpty(htmlString))
            {
                var list = ExtractEmails(htmlString);

                if (list != null && list.Count > 0)
                {
                    emails = string.Join(", ", list);
                }

            }

            return emails;
        }

        public string SearchEmails(string keyword, string website = "", int limit = 10, int offset = 0)
        {
            string emails = null;
            string queryString = BuildQueryStringGoogleSearch($"email of {keyword}");
            var htmlDocument = GetHtmlDocument(queryString);

            if (htmlDocument != null)
            {
                var sponsored = htmlDocument.DocumentNode.SelectNodes("//div[@class='ZINbbc O9g5cc uUPGi']"); // sponsored/adds
                var organic = htmlDocument.DocumentNode.SelectNodes("//div[@class='ZINbbc xpd O9g5cc uUPGi']"); // organic result

                // for sponsored/adds results
                if (sponsored != null)
                {
                    Debug.WriteLine("Fetching sponsored results:");
                    int counter = 1;
                    foreach (var result in organic)
                    {
                        string title = null;
                        string description = null;
                        string url = null;

                        try
                        {
                            var divTitle = result.SelectSingleNode(".//div[@class='BNeawe vvjwJb AP7Wnd']");
                            var divDescription = result.SelectSingleNode(".//div[@class='BNeawe s3v9rd AP7Wnd']");
                            var anchorUrl = result.SelectSingleNode(".//div[@class='kCrYT']/a");

                            title = divTitle != null ? divTitle.InnerText : null;
                            description = divDescription != null ? divDescription.InnerText : null;
                            url = anchorUrl != null ? CleanUrl(anchorUrl.GetAttributeValue("href", null)) : null;

                            Debug.WriteLine($"---->{counter}. Organic result:");
                            Debug.WriteLine($"-------->Title: {title}");
                            Debug.WriteLine($"-------->URL: {url}");
                            Debug.WriteLine($"-------->Extracting email from the URL...");

                            if (url != null)
                            {
                                bool suppressed = false;

                                if (url.ToLower().Contains("rocketreach"))
                                    suppressed = true;
                                else if (url.ToLower().Contains("tripadvisor"))
                                    suppressed = true;
                                else if (url.ToLower().Contains("glassdoor"))
                                    suppressed = true;
                                else if (url.ToLower().Contains("addressschool"))
                                    suppressed = true;
                                else if (url.ToLower().Contains("greatplacetowork"))
                                    suppressed = true;
                                else if (url.ToLower().Contains("zalora"))
                                    suppressed = true;
                                else if (url.ToLower().Contains("opentable"))
                                    suppressed = true;

                                if (suppressed)
                                {
                                    Debug.WriteLine($"-------->Could not extract email from the (suppressed) URL .");
                                    continue;
                                }
                            }
                            else
                            {
                                continue;
                            }

                            emails = GetEmailsFromWebsitePage(url);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"------------>Error while fetching organic result from: {url}. " + ex.Message);
                        }
                        finally
                        {
                            counter++;
                        }


                        if (emails != null)
                        {
                            if (!string.IsNullOrEmpty(website))
                            {
                                bool domainIsFoundInEmailAddress = false;

                                try
                                {
                                    string domain = ExtractDomainFromUrl(website);

                                    if (domain != null && emails.ToLower().Contains(domain.ToLower()))
                                    {
                                        domainIsFoundInEmailAddress = true;
                                    }

                                    if (!domainIsFoundInEmailAddress)
                                    {
                                        Debug.WriteLine($"------------>Email(s): [{emails}] does not match with the domain ({domain}). Trying to extract emails from the website: {website}");

                                        // 1st attempt
                                        string emailsFromWebsite = GetEmailsFromWebsitePage(website);


                                        if ((domain != null && emailsFromWebsite != null) && emailsFromWebsite.ToLower().Contains(domain.ToLower()))
                                        {
                                            Debug.WriteLine($"------------>Extracted email(s) from the website: [{emailsFromWebsite}]");
                                            emails = emailsFromWebsite;
                                        }

                                        if (string.IsNullOrEmpty(emailsFromWebsite))
                                        {
                                            Debug.WriteLine($"------------>No emails found from website: [{website}]");
                                            
                                            if (website != null)
                                            {
                                                if (!website.EndsWith("/"))
                                                {
                                                    website += "/";
                                                }

                                                // 2nd attempt
                                                emailsFromWebsite = GetEmailsFromWebsitePage(website + "contact-us");


                                                if ((domain != null && emailsFromWebsite != null) && emailsFromWebsite.ToLower().Contains(domain.ToLower()))
                                                {
                                                    Debug.WriteLine($"------------>Extracted email(s) from the website: [{emailsFromWebsite}]");
                                                    emails = emailsFromWebsite;
                                                }

                                                if (string.IsNullOrEmpty(emailsFromWebsite))
                                                {
                                                    Debug.WriteLine($"------------>No emails found from website: [{website}]");

                                                    //
                                                    emailsFromWebsite = GetEmailsFromWebsitePage(website + "contact");

                                           
                                                    if ((domain != null && emailsFromWebsite != null) && emailsFromWebsite.ToLower().Contains(domain.ToLower()))
                                                    {
                                                        Debug.WriteLine($"------------>Extracted email(s) from the website: [{emailsFromWebsite}]");
                                                        emails = emailsFromWebsite;
                                                    }

                                                    if (string.IsNullOrEmpty(emailsFromWebsite))
                                                    {
                                                        Debug.WriteLine($"------------>No emails found from website: [{website}]");
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Debug.WriteLine($"------------>Error while searching alternate email. " + ex.Message);
                                }
                            }



                            Debug.WriteLine($"------------>Email found: {emails}");
                            return emails.ToLower();
                        }
                        else
                        {
                            Debug.WriteLine($"------------>No email found from the organic result");
                        }
                    }

                }

                // for organic results
                if (organic != null)
                {
                    Debug.WriteLine("Fetching organic results:");
                    int counter = 1;
                    foreach (var result in organic)
                    {
                        string title = null;
                        string description = null;
                        string url = null;

                        try
                        {
                            var divTitle = result.SelectSingleNode(".//div[@class='BNeawe vvjwJb AP7Wnd']");
                            var divDescription = result.SelectSingleNode(".//div[@class='BNeawe s3v9rd AP7Wnd']");
                            var anchorUrl = result.SelectSingleNode(".//div[@class='kCrYT']/a");

                            title = divTitle != null ? divTitle.InnerText : null;
                            description = divDescription != null ? divDescription.InnerText : null;
                            url = anchorUrl != null ? CleanUrl(anchorUrl.GetAttributeValue("href", null)) : null;

                            Debug.WriteLine($"---->{counter}. Organic result:");
                            Debug.WriteLine($"-------->Title: {title}");
                            Debug.WriteLine($"-------->URL: {url}");
                            Debug.WriteLine($"-------->Extracting email from the URL...");

                            if (url != null)
                            {
                                bool suppressed = false;

                                if (url.ToLower().Contains("rocketreach"))
                                    suppressed = true;
                                else if (url.ToLower().Contains("tripadvisor"))
                                    suppressed = true;
                                else if (url.ToLower().Contains("glassdoor"))
                                    suppressed = true;
                                else if (url.ToLower().Contains("addressschool"))
                                    suppressed = true;
                                else if (url.ToLower().Contains("greatplacetowork"))
                                    suppressed = true;
                                else if (url.ToLower().Contains("zalora"))
                                    suppressed = true;
                                else if (url.ToLower().Contains("opentable"))
                                    suppressed = true;

                                if (suppressed)
                                {
                                    Debug.WriteLine($"-------->Could not extract email from the (suppressed) URL .");
                                    continue;
                                }
                            }
                            else
                            {
                                continue;
                            }

                            emails = GetEmailsFromWebsitePage(url);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"------------>Error while fetching organic result from: {url}. " + ex.Message);
                        }
                        finally
                        {
                            counter++;
                        }


                        if (emails != null)
                        {
                            if (!string.IsNullOrEmpty(website))
                            {
                                bool domainIsFoundInEmailAddress = false;

                                try
                                {
                                    string domain = ExtractDomainFromUrl(website);

                                    if (domain != null && emails.ToLower().Contains(domain.ToLower()))
                                    {
                                        domainIsFoundInEmailAddress = true;
                                    }

                                    if (!domainIsFoundInEmailAddress)
                                    {
                                        Debug.WriteLine($"------------>Email(s): [{emails}] does not match with the domain ({domain}). Trying to extract emails from the website: {website}");

                                        // 1st attempt
                                        string emailsFromWebsite = GetEmailsFromWebsitePage(website);


                                        if ((domain != null && emailsFromWebsite != null) && emailsFromWebsite.ToLower().Contains(domain.ToLower()))
                                        {
                                            Debug.WriteLine($"------------>Extracted email(s) from the website: [{emailsFromWebsite}]");
                                            emails = emailsFromWebsite;
                                        }

                                        if (string.IsNullOrEmpty(emailsFromWebsite))
                                        {
                                            Debug.WriteLine($"------------>No emails found from website: [{website}]");

                                            if (website != null)
                                            {
                                                if (!website.EndsWith("/"))
                                                {
                                                    website += "/";
                                                }

                                                try
                                                {
                                                    // 2nd attempt
                                                    emailsFromWebsite = GetEmailsFromWebsitePage(website + "contact-us");
                                                }
                                                catch (Exception ex)
                                                {
                                                    Debug.WriteLine("Error in second attempt extracting email from website: " + (website + "contact-us. ") + ex.Message);
                                                }


                                                if ((domain != null && emailsFromWebsite != null) && emailsFromWebsite.ToLower().Contains(domain.ToLower()))
                                                {
                                                    Debug.WriteLine($"------------>Extracted email(s) from the website: [{emailsFromWebsite}]");
                                                    emails = emailsFromWebsite;
                                                }
                                            }
                                        }

                                        if (string.IsNullOrEmpty(emailsFromWebsite))
                                        {
                                            Debug.WriteLine($"------------>No emails found from website: [{website}]");

                                            if (website != null)
                                            {
                                                if (!website.EndsWith("/"))
                                                {
                                                    website += "/";
                                                }

                                                try
                                                {
                                                    // 3rd attempt
                                                    emailsFromWebsite = GetEmailsFromWebsitePage(website + "contact");
                                                }
                                                catch (Exception ex)
                                                {
                                                    Debug.WriteLine("Error in second attempt extracting email from website: " + (website + "contact. ") + ex.Message);
                                                }


                                                if ((domain != null && emailsFromWebsite != null) && emailsFromWebsite.ToLower().Contains(domain.ToLower()))
                                                {
                                                    Debug.WriteLine($"------------>Extracted email(s) from the website: [{emailsFromWebsite}]");
                                                    emails = emailsFromWebsite;
                                                }
                                            }
                                        }

                                        if (string.IsNullOrEmpty(emailsFromWebsite))
                                        {
                                            Debug.WriteLine($"------------>No emails found from website: [{website}]");

                                            if (website != null)
                                            {
                                                if (!website.EndsWith("/"))
                                                {
                                                    website += "/";
                                                }

                                                try
                                                {
                                                    // 4th attempt
                                                    emailsFromWebsite = GetEmailsFromWebsitePage(website + "sample-page");
                                                }
                                                catch (Exception ex)
                                                {
                                                    Debug.WriteLine("Error in second attempt extracting email from website: " + (website + "sample-page. ") + ex.Message);
                                                }


                                                if ((domain != null && emailsFromWebsite != null) && emailsFromWebsite.ToLower().Contains(domain.ToLower()))
                                                {
                                                    Debug.WriteLine($"------------>Extracted email(s) from the website: [{emailsFromWebsite}]");
                                                    emails = emailsFromWebsite;
                                                }
                                            }
                                        }

                                        if (string.IsNullOrEmpty(emailsFromWebsite))
                                        {
                                            Debug.WriteLine($"------------>No emails found from website: [{website}]");

                                            if (website != null)
                                            {
                                                if (!website.EndsWith("/"))
                                                {
                                                    website += "/";
                                                }

                                                try
                                                {
                                                    // 5th attempt
                                                    emailsFromWebsite = GetEmailsFromWebsitePage(website + "about");
                                                }
                                                catch (Exception ex)
                                                {
                                                    Debug.WriteLine("Error in second attempt extracting email from website: " + (website + "about. ") + ex.Message);
                                                }


                                                if ((domain != null && emailsFromWebsite != null) && emailsFromWebsite.ToLower().Contains(domain.ToLower()))
                                                {
                                                    Debug.WriteLine($"------------>Extracted email(s) from the website: [{emailsFromWebsite}]");
                                                    emails = emailsFromWebsite;
                                                }
                                            }
                                        }

                                        if (string.IsNullOrEmpty(emailsFromWebsite))
                                        {
                                            Debug.WriteLine($"------------>No emails found from website: [{website}]");

                                            if (website != null)
                                            {
                                                if (!website.EndsWith("/"))
                                                {
                                                    website += "/";
                                                }

                                                try
                                                {
                                                    // 6th attempt
                                                    emailsFromWebsite = GetEmailsFromWebsitePage(website + "about-us");
                                                }
                                                catch (Exception ex)
                                                {
                                                    Debug.WriteLine("Error in second attempt extracting email from website: " + (website + "about-us. ") + ex.Message);
                                                }


                                                if ((domain != null && emailsFromWebsite != null) && emailsFromWebsite.ToLower().Contains(domain.ToLower()))
                                                {
                                                    Debug.WriteLine($"------------>Extracted email(s) from the website: [{emailsFromWebsite}]");
                                                    emails = emailsFromWebsite;
                                                }
                                            }
                                        }




                                    }
                                }
                                catch (Exception ex)
                                {
                                    Debug.WriteLine($"------------>Error while searching alternate email. " + ex.Message);
                                }
                            }


                            // filter emails relevant to the domain
                            string[] emailArr = emails.Split(',');

                            if (emailArr.Length > 0)
                            {
                                List<string> emailList = new List<string>();

                                string domain = ExtractDomainFromUrl(website);

                                if (domain != null)
                                {
                                    foreach (var e in emailArr)
                                    {
                                        if (e.ToLower().Contains(domain.ToLower()))
                                        {
                                            emailList.Add(e.Trim());
                                        }
                                    }

                                    if (emailList.Count > 0)
                                    {
                                        emails = string.Join(", ", emailList);
                                    }
                                }

                                
                            }


                            Debug.WriteLine($"------------>Email found: {emails}");
                            return emails.ToLower();
                        }
                        else
                        {
                            Debug.WriteLine($"------------>No email found from the organic result");
                        }
                    }
                } // end organic search
                    
            }

            return emails;
        }


        private static string ExtractDomainFromUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return null;
            }

            string domain = url;

            if (domain.Contains(@"://"))
            {
                domain = url.Split(new string[] { "://" }, 2, StringSplitOptions.None)[1];
            }

            return domain.Split('/')[0].Replace("www.", string.Empty);
        }

        public static List<string> ExtractEmails(string text)
        {
            text = text.Replace("<div>", string.Empty);
            text = text.Replace("</div>", string.Empty);
            text = text.Replace("<a>", string.Empty);
            text = text.Replace("</a>", string.Empty);
            text = text.Replace("<span>", string.Empty);
            text = text.Replace("</span>", string.Empty);
            text = text.Replace("class=", string.Empty);
            text = text.Replace("id=", string.Empty);
            text = text.Replace("<script>", string.Empty);
            text = text.Replace("</script>", string.Empty);
            text = text.Replace("<li>", string.Empty);
            text = text.Replace("</li>", string.Empty);
            text = text.Replace("<ul>", string.Empty);
            text = text.Replace("</ul>", string.Empty);


            const string MatchEmailPattern = @"(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
           + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
             + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
           + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})";
            Regex rx = new Regex(MatchEmailPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            // Find matches.
            MatchCollection matches = rx.Matches(text);
            // Report the number of matches found.
            int noOfMatches = matches.Count;
            // Report on each match.

            List<string> emails = new List<string>();

            foreach (Match match in matches)
            {
                //Debug.WriteLine(match.Value.ToString());
                if (!String.IsNullOrEmpty(match.Value))
                {
                    var email = match.Value;

                    if (email.Contains("wix.com"))
                        continue;
                    else if (email.Contains("wixpress.com"))
                        continue;
                    else if (email.Contains("sentry.io"))
                        continue;
                    else if (email.Contains("sentry.glassdoor.com"))
                        continue;
                    else if (email.Contains("glassdoor.com"))
                        continue;
                    else if (email.Contains("greatplacetowork.com"))
                        continue;


                    emails.Add(match.Value);
                }
            }


            return emails.Distinct().ToList();
        }

        private static string BuildQueryStringGoogleSearch(string keyword)
        {
            return @"https://www.google.com/search?q=" + HttpUtility.UrlEncode(keyword);
        }

        private static string ExtractAddressAndContactInfo(HtmlNode node, string label)
        {
            string text = null;
            try
            {
                var addressAndContactSection = node.SelectNodes("//div[@class='vbShOe kCrYT']/div[contains(normalize-space(@class), 'AVsepf')]/div[@class='BNeawe s3v9rd AP7Wnd']");

                if (addressAndContactSection != null)
                {
                    foreach (var div in addressAndContactSection)
                    {
                        var spans = div.SelectNodes("span");

                        if (spans != null && spans.Count >= 2)
                        {
                            // get label from first span
                            if (label.ToLower().Contains(spans[0].InnerText.ToLower()))
                            {
                                // get value from the second span
                                text = System.Net.WebUtility.HtmlDecode(spans[1].InnerText);
                                break;
                            }
                        }
                    }
                    
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return text;
        }

        private static string ExtractWebsite(HtmlNode node)
        {
            string hrefValue = null;
            var linksNode = node.SelectSingleNode("//div[@class='skVgpb']");
            
            if (linksNode != null && linksNode.HasChildNodes)
            {
                var anchors = linksNode.SelectNodes("//a[@class='VGHMXd']");

                if (anchors != null && anchors.Count >= 2)
                {
                    hrefValue = GetAttributeValue("href", anchors[1]); // website is always the second anchor tag
                    hrefValue = CleanUrl(hrefValue);
                }
            }

            return hrefValue;
        }

        private static string CleanUrl(string text)
        {
            if (!String.IsNullOrEmpty(text))
            {
                text = System.Net.WebUtility.HtmlDecode(text.Replace("/url?q=", String.Empty));
                int index = text.IndexOf("&sa");

                if (index > 0)
                    text = text.Substring(0, index);
            }

            return text;
        }

        private static string CleanReviews(string text)
        {
            if (!String.IsNullOrEmpty(text) && (text.Contains('(') || text.Contains(')')))
            {
                text = text.Replace("(", String.Empty).Replace(")", String.Empty);

                
            }

            return text;
        }

        private static string CleanDescription(string text, string rating, string reviews)
        {
            if (!String.IsNullOrEmpty(text))
            {
                if (!String.IsNullOrEmpty(rating))
                {
                    text = text.Replace(rating, String.Empty);
                }

                if (!String.IsNullOrEmpty(reviews))
                {
                    text = text.Replace(reviews, String.Empty);
                }

                return text.Trim();
            }

            return text;
        }

        private static HtmlDocument GetHtmlDocument(string url)
        {
            var client = new System.Net.WebClient();
            byte[] buffer = client.DownloadData(url);

            var htmlDocument = new HtmlAgilityPack.HtmlDocument();
            htmlDocument.LoadHtml(client.DownloadString(url));

            return htmlDocument;

            /*
            using (var streamReader = new StreamReader(WebRequest.Create(url).GetResponse().GetResponseStream()))
            {
                var htmlString = streamReader.ReadToEnd();

                if (!String.IsNullOrEmpty(htmlString))
                {
                    var htmlDocument = new HtmlAgilityPack.HtmlDocument();
                    htmlDocument.LoadHtml(htmlString);

                    return htmlDocument;
                }
            }
            

            return null;
            */
        }

        private static string GetHtmlString(string url)
        {
            var client = new System.Net.WebClient();
            byte[] buffer = client.DownloadData(url);

            var htmlDocument = new HtmlAgilityPack.HtmlDocument();
            return client.DownloadString(url);
        }

        private static string GetAttributeValue(string attributeName, HtmlNode node, string path = "")
        {
            try
            {
                if (node != null)
                {
                    if (String.IsNullOrEmpty(path))
                        return System.Net.WebUtility.HtmlDecode(node.GetAttributeValue(attributeName, null));

                    return System.Net.WebUtility.HtmlDecode(node
                        .SelectSingleNode(path)
                        .GetAttributeValue(attributeName, null));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return String.Empty;
        }

        private static string GetInnerText(HtmlNode node, string path)
        {
            try
            {
                if (node != null)
                {
                    var n = node.SelectSingleNode(path);

                    if (n != null)
                        return System.Net.WebUtility.HtmlDecode(node.SelectSingleNode(path).InnerText);
                }
                   
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return String.Empty;
        }


    }
}
