var articles; // Array of ViewModels (which have AsView() method overridden), server should feed this array
var per_page; // Number of articles to display on one page, server shoud feed this value

var pagination_section, pagination_list, article_list, active = 0;

doc.ready(function()
{
	if (typeof articles === "undefined") articles = [];
	if (typeof per_page === "undefined") per_page = 10;
	
	pagination_section = $("section#pagination");
	pagination_list = pagination_section.find("ul#pagination-list");
	article_list = pagination_section.find("div#article-list");
	
	var pagination_pages = articles.length / per_page;
	if (articles.length % per_page !== 0) ++pagination_pages;
	
	for (var i = 1; i <= pagination_pages; i++)
	{
		pagination_list.append("<li><a class=\"cursor-pointer\" onclick=\"activatePage(" + i + ");\">" + i + "</a></li>");
	}
	
	activatePage(1);
});

function activatePage(page)
{
	if (page === active) return;
	
	article_list.html("");
	
	for (var i = (page - 1) * per_page; i < articles.length && i < page * per_page; ++i)
	{
		article_list.append(articles[i].AsView());
	}
	
	pagination_list.find("li.active").removeClass("active");
	pagination_list.find("li:nth-child(" + page + ")").addClass("active");
	active = page;
}