//
// TabStrip.cs
//
// Author:
//   Lluis Sanchez Gual
//

//
// Copyright (C) 2007 Novell, Inc (http://www.novell.com)
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

using Gtk; 

using System;
using System.Linq;
using Hexadock;
using Xwt.Drawing;

namespace Pinta.Docking
{
	class TabStrip: Gtk.EventBox
	{
		int currentTab = -1;
		HBox box = new HBox ();
		Label bottomFiller = new Label ();
		DockVisualStyle visualStyle;

		public TabStrip (DockFrame frame)
		{
			VBox vbox = new VBox ();
			box = new TabStripBox () { TabStrip = this };
			vbox.PackStart (box, false, false, 0);
		//	vbox.PackStart (bottomFiller, false, false, 0);
			Add (vbox);
			ShowAll ();
			bottomFiller.Hide ();
			BottomPadding = 3;
			WidthRequest = 0;
			box.Removed += HandleRemoved;
		}

		public int BottomPadding {
			get { return bottomFiller.HeightRequest; }
			set {
				bottomFiller.HeightRequest = value;
				bottomFiller.Visible = value > 0;
			}
		}

		public DockVisualStyle VisualStyle {
			get { return visualStyle; }
			set {
				visualStyle = value;
				box.QueueDraw ();
			}
		}
		
		public void AddTab (DockItemTitleTab tab)
		{
			if (tab.Parent != null)
				((Gtk.Container)tab.Parent).Remove (tab);

			//box.PackStart (tab, true, true, 0);
			box.PackStart (tab, false, false, 0);
			tab.WidthRequest = tab.LabelWidth;
			if (currentTab == -1)
				CurrentTab = box.Children.Length - 1;
			else {
				tab.Active = false;
				tab.Page.Hide ();
			}
			
			tab.ButtonPressEvent += OnTabPress;
		}

		void HandleRemoved (object o, RemovedArgs args)
		{
			Gtk.Widget w = args.Widget;
			w.ButtonPressEvent -= OnTabPress;
			if (currentTab >= box.Children.Length)
				currentTab = box.Children.Length - 1;
		}

        public void SetTabLabel (Gtk.Widget page, Gdk.Pixbuf icon, string label)
		{
			foreach (DockItemTitleTab tab in box.Children) {
				if (tab.Page == page) {
					tab.SetLabel (page, icon, label);
					UpdateEllipsize (Allocation);
					break;
				}
			}
		}
		
		public void UpdateStyle (DockItem item)
		{
			QueueResize ();
		}

		public int TabCount {
			get { return box.Children.Length; }
		}
		
		public int CurrentTab {
			get { return currentTab; }
			set {
				if (currentTab == value)
					return;
				if (currentTab != -1) {
					DockItemTitleTab t = (DockItemTitleTab) box.Children [currentTab];
					t.Page.Hide ();
					t.Active = false;
				}
				currentTab = value;
				if (currentTab != -1) {
					DockItemTitleTab t = (DockItemTitleTab) box.Children [currentTab];
					t.Active = true;
					t.Page.Show ();
				}
			}
		}
		
		public Gtk.Widget CurrentPage {
			get {
				if (currentTab != -1) {
					DockItemTitleTab t = (DockItemTitleTab) box.Children [currentTab];
					return t.Page;
				} else
					return null;
			}
			set {
				if (value != null) {
					Gtk.Widget[] tabs = box.Children;
					for (int n = 0; n < tabs.Length; n++) {
						DockItemTitleTab tab = (DockItemTitleTab) tabs [n];
						if (tab.Page == value) {
							CurrentTab = n;
							return;
						}
					}
				}
				CurrentTab = -1;
			}
		}
		
		public void Clear ()
		{
			currentTab = -1;
			foreach (DockItemTitleTab w in box.Children)
				box.Remove (w);
		}
		
		void OnTabPress (object s, Gtk.ButtonPressEventArgs args)
		{
			CurrentTab = Array.IndexOf (box.Children, s);
			DockItemTitleTab t = (DockItemTitleTab) s;
			DockItem.SetFocus (t.Page);
			QueueDraw ();
			args.RetVal = true;
		}

		protected override void OnSizeAllocated (Gdk.Rectangle allocation)
		{
			UpdateEllipsize (allocation);
			base.OnSizeAllocated (allocation);
		}
		
		void UpdateEllipsize (Gdk.Rectangle allocation)
		{
			int tabsSize = 0;
			var children = box.Children;

			foreach (DockItemTitleTab tab in children)
				tabsSize += tab.LabelWidth;

			var totalWidth = allocation.Width;

			int[] sizes = new int[children.Length];
			double ratio = (double) allocation.Width / (double) tabsSize;

			if (ratio > 1 && visualStyle.ExpandedTabs.Value) {
				// The tabs have to fill all the available space. To get started, assume that all tabs with have the same size 
				var tsize = totalWidth / children.Length;
				// Maybe the assigned size is too small for some tabs. If it happens the extra space it requires has to be taken
				// from tabs which have surplus of space. To calculate it, first get the difference beteen the assigned space
				// and the required space.
				for (int n=0; n<children.Length; n++)
					sizes[n] = tsize - ((DockItemTitleTab)children[n]).LabelWidth;

				// If all is positive, nothing is left to do (all tabs have enough space). If there is any negative, it means
				// that space has to be reassigned. The negative space has to be turned into positive by reducing space from other tabs
				for (int n=0; n<sizes.Length; n++) {
					if (sizes[n] < 0) {
						ReduceSizes (sizes, -sizes[n]);
						sizes[n] = 0;
					}
				}
				// Now calculate the final space assignment of each tab
				for (int n=0; n<children.Length; n++) {
					sizes[n] += ((DockItemTitleTab)children[n]).LabelWidth;
					totalWidth -= sizes[n];
				}
			} else {
				if (ratio > 1)
					ratio = 1;
				for (int n=0; n<children.Length; n++) {
					var s = (int)((double)((DockItemTitleTab)children[n]).LabelWidth * ratio);
					sizes[n] = s;
					totalWidth -= s;
				}
			}

			// There may be some remaining space due to rounding. Spread it
			for (int n=0; n<children.Length && totalWidth > 0; n++) {
				sizes[n]++;
				totalWidth--;
			}
			// Assign the sizes
			for (int n=0; n<children.Length; n++)
				children[n].WidthRequest = sizes[n];
		}

		void ReduceSizes (int[] sizes, int amout)
		{
			// Homogeneously removes 'amount' pixels from the array of sizes, making sure
			// no size goes below 0.
			while (amout > 0) {
				int part;
				int candidates = sizes.Count (s => s > 0);
				if (candidates == 0)
					return;
				part = Math.Max (amout / candidates, 1);

				for (int n=0; n<sizes.Length && amout > 0; n++) {
					var s = sizes [n];
					if (s <= 0) continue;
					if (s > part) {
						s -= part;
						amout -= part;
					} else {
						amout -= s;
						s = 0;
					}
					sizes[n] = s;
				}
			}
		}

		internal class TabStripBox: HBox
		{
			public TabStrip TabStrip;

			protected override bool OnDrawn (Cairo.Context cr)
			{
				if (TabStrip.VisualStyle.TabStyle == DockTabStyle.Normal) {
					var alloc = Allocation;
					Gdk.CairoHelper.Rectangle(cr, alloc);
					cr.Clip();
					
					cr.SetSourceColor(TabStrip.VisualStyle.InactivePadBackgroundColor.Value);
					cr.Paint();
					cr.LineWidth = 1;
					cr.SetSourceColor(Styles.ThinSplitterColor.ToCairoColor());
					
					//
					// Gdk.RGBA bgc = new Gdk.RGBA ();
					// var c = (HslColor) TabStrip.VisualStyle.PadBackgroundColor.Value;
					// c.L *= 0.7;
					// bgc = c;
					// Gdk.CairoHelper.SetSourceRgba(cr, bgc);
					// cr.Line (alloc.X, alloc.Y + alloc.Height - 1, alloc.X + alloc.Width - 1, alloc.Y + alloc.Height - 1);
					// cr.Stroke();
					cr.Line(0, Allocation.Height, Allocation.Width, Allocation.Height);
					cr.Stroke();

					foreach (var tab in from w in TabStrip.box.Children
						where w is DockItemTitleTab && ((DockItemTitleTab) w).Active
						select (DockItemTitleTab) w)
					{
						Gdk.CairoHelper.Rectangle(cr, tab.Allocation);
						cr.Clip();
						Gdk.CairoHelper.SetSourceRgba(cr, TabStrip.VisualStyle.PadBackgroundColor.Value);
						cr.Paint();

						cr.Line(tab.Allocation.X, Allocation.Height, tab.Allocation.X + tab.Allocation.Width,
							Allocation.Height);
						cr.Stroke();

						cr.SetSourceColor(Styles.ThinSplitterColor.ToCairoColor());
						cr.Line(tab.Allocation.X, 0, tab.Allocation.X, tab.Allocation.Height);
						cr.Line(tab.Allocation.X + tab.Allocation.Width, 0, tab.Allocation.X + tab.Allocation.Width,
							tab.Allocation.Height);
						cr.Stroke();
						cr.ResetClip();
					}
				}

				return base.OnDrawn (cr);
			}
		}
		
	}
	
}

