namespace !NAMESPACE!.Web.Controllers
{
	using Castle.MonoRail.Framework;

	[Layout("default"), Rescue("generalerror")]
	public abstract class BaseController : SmartDispatcherController
	{

	}
}
