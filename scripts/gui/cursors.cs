//-----------------------------------------------------------------------------
// Copyright (c) 2012 GarageGames, LLC
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to
// deal in the Software without restriction, including without limitation the
// rights to use, copy, modify, merge, publish, distribute, sublicense, and/or
// sell copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
// IN THE SOFTWARE.
//-----------------------------------------------------------------------------

if($platform $= "macos")
{
   new GuiCursor(DefaultCursor)
   {
      hotSpot = "4 4";
      renderOffset = "0 0";
      bitmapName = "~/art/gui/images/macCursor";
   };
} 
else 
{
   new GuiCursor(DefaultCursor)
   {
      hotSpot = "1 1";
      renderOffset = "0 0";
      bitmapName = "~/art/gui/images/CUR_3darrow";
   };
}

new GuiCursor(CurArrow)
{
   hotSpot = "1 1";
   renderOffset = "0 0";
   bitmapName = "~/art/gui/images/CUR_3darrow";
};

//FIXME CurWait
//FIXME CurPlus

new GuiCursor(CurResizeVert)
{
   hotSpot = "1 1";
   renderOffset = "0.5 0.1";
   bitmapName = "~/art/gui/images/CUR_leftright";
};

new GuiCursor(CurResizeHorz)
{
   hotSpot = "1 1";
   renderOffset = "0.5 0.5";
   bitmapName = "~/art/gui/images/CUR_updown";
};

new GuiCursor(CurResizeAll)
{
   hotSpot = "1 1";
   renderOffset = "0.5 0.5";
   bitmapName = "~/art/gui/images/CUR_move";
};


new GuiCursor(CurIBeam)
{
   hotSpot = "1 1";
   renderOffset = "0.5 0.5";
   bitmapName = "~/art/gui/images/CUR_textedit";
};

new GuiCursor(CurResizeNESW)
{
   hotSpot = "1 1";
   renderOffset = "0.5 0.5";
   bitmapName = "~/art/gui/images/CUR_nesw";
};

new GuiCursor(CurResizeNWSE)
{
   hotSpot = "1 1";
   renderOffset = "0.5 0.5";
   bitmapName = "~/art/gui/images/CUR_nwse";
};

//FIXME CurHand
//FIXME CurWaitArrow
//FIXME CurNoNo

