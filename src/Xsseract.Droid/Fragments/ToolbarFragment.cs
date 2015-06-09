using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.OS;
using Android.Views;
using com.refractored.fab;

namespace Xsseract.Droid.Fragments
{
  // TODO: Icons not suggestive enough.
  public class ToolbarFragment : Fragment
  {
    private FloatingActionButton fabCrop;
    private FloatingActionButton fabCamera;
    private FloatingActionButton fabAccept;
    private FloatingActionButton fabToClipboard;
    private FloatingActionButton fabShare;

    private List<FloatingActionButton> allFabs;

    public event EventHandler<EventArgs> Camera;
    public event EventHandler<EventArgs> Crop;
    public event EventHandler<EventArgs> CopyToClipboard;
    public event EventHandler<EventArgs> Share;
    public event EventHandler<EventArgs> Accept;

    public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
    {
      return inflater.Inflate(Resource.Layout.Toolbar, null);
    }

    public override void OnViewCreated(View view, Bundle savedInstanceState)
    {
      base.OnViewCreated(view, savedInstanceState);

      fabCamera = view.FindViewById<FloatingActionButton>(Resource.Id.fabCamera);
      fabCrop = view.FindViewById<FloatingActionButton>(Resource.Id.fabCrop);
      fabAccept = view.FindViewById<FloatingActionButton>(Resource.Id.fabAccept);
      fabToClipboard = view.FindViewById<FloatingActionButton>(Resource.Id.fabToClipboard);
      fabShare = view.FindViewById<FloatingActionButton>(Resource.Id.fabShare);

      HideFab(fabToClipboard, false);
      HideFab(fabShare, false);

      allFabs = new List<FloatingActionButton> { fabCamera, fabCrop, fabAccept, fabToClipboard, fabShare };

      fabCamera.Click += fabCamera_Click;
      fabCrop.Click += fabCrop_Click;
      fabAccept.Click += fabAccept_Click;
      fabToClipboard.Click += (sender, e) => OnCopyToClipboard(EventArgs.Empty);
      fabShare.Click += (sender, e) => OnShare(EventArgs.Empty);
    }

    public void ShowCroppingTools(bool animate)
    {
      SetVisibleFabs(fabCrop, fabCamera);
    }

    public void ShowResultTools(bool animate)
    {
      SetVisibleFabs(fabToClipboard, fabShare);
    }

    public void ShowResultToolsNoShare(bool animate)
    {
      SetVisibleFabs(fabAccept);
    }

    public void HideAll()
    {
      SetVisibleFabs();
    }

    protected void OnCrop(EventArgs e)
    {
      var handler = Crop;
      if (null != handler)
      {
        handler(this, e);
      }
    }

    protected void OnCamera(EventArgs e)
    {
      var handler = Camera;
      if (null != handler)
      {
        handler(this, e);
      }
    }

    protected void OnShare(EventArgs e)
    {
      var handler = Share;
      if(null != handler)
      {
        handler(this, e);
      }
    }

    protected void OnAccept(EventArgs e)
    {
      var handler = Accept;
      if(null != handler)
      {
        handler(this, e);
      }
    }

    protected void OnCopyToClipboard(EventArgs e)
    {
      var handler = CopyToClipboard;
      if(null != handler)
      {
        handler(this, e);
      }
    }

    private void fabCrop_Click(object sender, EventArgs eventArgs)
    {
      OnCrop(EventArgs.Empty);
    }

    private void fabCamera_Click(object sender, EventArgs e)
    {
      OnCamera(EventArgs.Empty);
    }

    private void fabAccept_Click(object sender, EventArgs eventArgs)
    {
      OnAccept(EventArgs.Empty);
    }

    private void SetVisibleFabs(params FloatingActionButton[] visibleFabs)
    {
      if (null == visibleFabs)
      {
        visibleFabs = new FloatingActionButton[0];
      }

      foreach (var f in allFabs)
      {
        if (!visibleFabs.Contains(f))
        {
          if (f.Visible)
          {
            HideFab(f, true);
          }
        }
        else
        {
          if (!f.Visible)
          {
            ShowFab(f, true);
          }
        }
      }
    }

    private void ShowFab(FloatingActionButton button, bool animate)
    {
      button.Visibility = ViewStates.Visible;
      button.Show(animate);
    }

    private void HideFab(FloatingActionButton button, bool animate)
    {
      button.Hide(animate);
      button.Visibility = ViewStates.Gone;
    }
  }
}