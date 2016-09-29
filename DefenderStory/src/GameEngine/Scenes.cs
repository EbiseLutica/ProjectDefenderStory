namespace TakeUpJewel.src
{
	internal abstract class Scene
	{
		public string Name { get; protected set; }


		public virtual void OnInit()
		{
		}

		public virtual void OnUpdate()
		{
		}

		public virtual void OnDestroy()
		{
		}
	}
}