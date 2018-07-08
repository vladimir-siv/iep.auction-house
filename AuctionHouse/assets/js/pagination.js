var articles = []; // Array of ViewModels (which have AsView() method overridden), server should feed this array
var per_page = 10; // Number of articles to display on one page, server shoud feed this value

var pagination_section, pagination_list, article_list, active = 0;

doc.ready(function()
{
	pagination_section = $("section#pagination");
	pagination_list = pagination_section.find("ul#pagination-list");
	article_list = pagination_section.find("div#article-list");
});

function setPaginationArticles(pagarticles)
{
	// Check diff for optimization
	if (articles.length === pagarticles.length && articles.length > 0)
	{
		var same = true;

		for (var i = 0; same && i < articles.length; ++i)
		{
			same = articles[i] === pagarticles[i];
		}
		
		if (same) return;
	}

	articles = pagarticles;
	initPagination();
}

function initPagination()
{
	if (active === 0) active = 1;

	var pagination_pages = Math.floor(articles.length / per_page);
	if (articles.length % per_page !== 0) ++pagination_pages;

	if (pagination_pages == 0) pagination_pages = 1;

	pagination_list.html("");
	for (var i = 1; i <= pagination_pages; ++i)
	{
		pagination_list.append("<li><a class=\"cursor-pointer\" onclick=\"if (" + i + " !== active) activatePage(" + i + ");\">" + i + "</a></li>");
	}

	if (active > pagination_pages) active = pagination_pages;
	activatePage(active);
}

function activatePage(page)
{
	if (page === 0) return;

	article_list.html("");
	
	for (var i = (page - 1) * per_page; i < articles.length && i < page * per_page; ++i)
	{
		article_list.append(articles[i].AsView());
	}
	
	pagination_list.find("li.active").removeClass("active");
	pagination_list.find("li:nth-child(" + page + ")").addClass("active");
	active = page;
}