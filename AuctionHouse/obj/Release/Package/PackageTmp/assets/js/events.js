class XEvent
{
	constructor()
	{
		this.callbacks = [];
	}

	reg(callback)
	{
		this.callbacks.push(callback);
	}

	unreg(callback)
	{
		var index = this.callbacks.lastIndexOf(callback);
		if (index >= 0) this.callbacks.splice(index, 1);
	}

	clear()
	{
		this.callbacks = [];
	}

	fire(sender, args)
	{
		for (var i = 0; i < this.callbacks.length; ++i)
		{
			this.callbacks[i](sender, args);
		}
	}
}