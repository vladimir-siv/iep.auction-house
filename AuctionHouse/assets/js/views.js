class ViewModel
{
	AsView() { return ""; }
}

class DynamicViewModel extends ViewModel
{
	constructor()
	{
		super();
		
		this.holder = null;
		this.dynamic = {};
		this.setupNeeded = false;
	}
	
	register(holder)
	{
		this.holder = holder;
	}
	
	fetch(type, name)
	{
		if (this.holder === null) return null;
		this.dynamic[type + "-" + name] = this.holder.find(type + "[data-dynamic='" + name + "']");
		
		var component = this.dynamic[type + "-" + name];
		if (typeof compnent === "undefined") return null;
		return component;
	}
	
	dynamics(type, name)
	{
		if (this.setupNeeded) this.Setup();
		var component = this.dynamic[type + "-" + name];
		if (typeof component === "undefined") return null;
		return component;
	}
	
	dynamics(type, name, value)
	{
		if (this.setupNeeded) this.Setup();
		var component = this.dynamic[type + "-" + name];
		if (typeof component === "undefined") return null;
		component.html(value);
		return component;
	}
	
	Setup() { this.setupNeeded = false; }
	
	AsView()
	{
		this.setupNeeded = true;
		return super.AsView();
	}
}