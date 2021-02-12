//
// ArrowWindow.cs
//
// Author:
//   Lluis Sanchez Gual
//
// Copyright (C) 2005 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using Cairo;
using Gtk;
using Hexadock;
using Point = Gdk.Point;

namespace Pinta.Docking.DockToolbars
{
	internal class ArrowWindow: Gtk.Window
	{
		const int LineWidth = 3;
		const int LineLength = 9;
		const int PointerWidth = 13;
		const int PointerLength = 6;
		
		Direction direction;
		Point[] arrow;
		int width, height;
		
		Gdk.RGBA redgc;
		
		// Where does the arrow point to
		public new enum Direction {
			Up, Down, Left, Right
		}
		
		public ArrowWindow (DockToolbarFrame frame, Direction dir): base (Gtk.WindowType.Popup)
		{
			SkipTaskbarHint = true;
			Decorated = false;
			TransientFor = frame.TopWindow;

			direction = dir;
			arrow = CreateArrow ();
			if (direction == Direction.Up || direction == Direction.Down) {
				 width = PointerWidth;
				 height = LineLength + PointerLength + 1;
			} else {
				 height = PointerWidth;
				 width = LineLength + PointerLength + 1;
			}
			
			// Create the mask for the arrow
			Gdk.RGBA black, white;
			black = new Gdk.RGBA ();
			black.Red = black.Green = black.Blue = 0;
			black.Alpha = 1;
			white = new Gdk.RGBA ();
			white.Red = white.Green = white.Blue = 1;
			white.Alpha = 1;
			
			using (Cairo.Surface pm = new ImageSurface(Format.ARGB32, width, height))
			using (Cairo.Context cr = new Cairo.Context(pm))
			{
				Gdk.CairoHelper.SetSourceRgba(cr, white);
				cr.Rectangle (0, 0, width, height);
				cr.Fill();

				cr.GdkPolygon(arrow);
				cr.Fill();
				Gdk.CairoHelper.SetSourceRgba(cr, black);
				cr.StrokePreserve();
				
				using (var region = Gdk.CairoHelper.RegionCreateFromSurface(pm))
					this.ShapeCombineRegion(region);
			}
			
			Realize ();

			redgc = new Gdk.RGBA
			{
				Red = 1,
				Green = 0,
				Blue = 0
			};
			
			Resize (width, height);
		}
		
		public int Width {
			get { return width; }
		}
		
		public int Height {
			get { return height; }
		}
		
		Point[] CreateArrow ()
		{
			Point[] ps = new Point [8];
			ps [0] = GetPoint (0, (PointerWidth/2) - (LineWidth/2));
			ps [1] = GetPoint (LineLength, (PointerWidth/2) - (LineWidth/2));
			ps [2] = GetPoint (LineLength, 0);
			ps [3] = GetPoint (PointerLength + LineLength, (PointerWidth/2));
			ps [4] = GetPoint (LineLength, PointerWidth - 1);
			ps [5] = GetPoint (LineLength, (PointerWidth/2) + (LineWidth/2));
			ps [6] = GetPoint (0, (PointerWidth/2) + (LineWidth/2));
			ps [7] = ps [0];
			return ps;
		}
		
		Point GetPoint (int x, int y)
		{
			switch (direction) {
				case Direction.Right: return new Point (x, y);
				case Direction.Left: return new Point ((PointerLength + LineLength) - x, y);
				case Direction.Down: return new Point (y, x);
				default: return new Point (y, (PointerLength + LineLength) - x);
			}
		}
		
		protected override bool OnDrawn (Cairo.Context cr)
		{
			Gdk.CairoHelper.SetSourceRgba(cr, redgc);
			cr.GdkPolygon(arrow);
			cr.StrokePreserve();
			cr.Fill();
			return true;
		}
	}
}
