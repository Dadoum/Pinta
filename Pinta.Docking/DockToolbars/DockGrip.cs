//
// DockGrip.cs
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
using Gtk;
using Gdk;
using Pinta.Docking;

namespace Pinta.Docking.DockToolbars
{
	internal class DockGrip: ToolItem
	{
		static int GripSize = Platform.IsWindows? 4 : 6; //wimp theme engine looks ugly with width 6
		new const int MarginLeft = 1;
		new const int MarginRight = 3;
		
		public DockGrip ()
		{
		}

		protected override void OnGetPreferredWidth(out int minimum_width, out int natural_width)
		{
			base.OnGetPreferredWidth(out minimum_width, out natural_width);
			if (Orientation == Orientation.Horizontal)
				minimum_width = natural_width = GripSize + MarginLeft + MarginRight;
			else
				minimum_width = natural_width = 0;
		}

		protected override void OnGetPreferredHeight(out int minimum_height, out int natural_height)
		{
			base.OnGetPreferredHeight(out minimum_height, out natural_height);
			if (Orientation == Orientation.Horizontal)
				minimum_height = natural_height = 0;
			else
				minimum_height = natural_height = GripSize + MarginLeft + MarginRight;
		}

		protected override bool OnDrawn (Cairo.Context cr)
		{
			Rectangle rect = Allocation;
			if (Orientation == Orientation.Horizontal) {
				rect.Width = GripSize;
				rect.X += MarginLeft;
			} else {
				rect.Height = GripSize;
				rect.Y += MarginLeft;
			}
			
			StyleContext.RenderHandle(cr, rect.X, rect.Y, rect.Width, rect.Height);
			return true;
		}
	}
}
