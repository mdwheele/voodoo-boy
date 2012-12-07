using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Demina
{
	public class TextureBounds
	{
		public Rectangle Location { get; set; }
		public Vector2 Origin { get; set; }

		public TextureBounds()
		{
		}

		public TextureBounds(Rectangle location, Vector2 origin)
		{
			Location = location;
			Origin = origin;
		}
	}
}
