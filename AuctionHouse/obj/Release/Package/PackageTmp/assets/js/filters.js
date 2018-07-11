class XFilter
{
	constructor()
	{
		this.filters = [];
	}

	reg(filter)
	{
		this.filters.push(filter);
	}

	unreg(filter)
	{
		var index = this.filters.lastIndexOf(filter);
		if (index >= 0) this.filters.splice(index, 1);
	}

	clear()
	{
		this.filters = [];
	}

	check(object)
	{
		for (var i = 0; i < this.filters.length; ++i)
		{
			if (!this.filters[i](object)) return false;
		}

		return true;
	}
}