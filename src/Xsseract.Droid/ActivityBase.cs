#region

using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Xsseract.Droid.Fragments;
using Environment = System.Environment;
using Uri = Android.Net.Uri;

#endregion

namespace Xsseract.Droid
{
  public class ActivityBase : FragmentActivity
  {
    #region Fields

    private ProgressDialog progressDialog;
    private ToolbarFragment toolbar;

    #endregion

    protected XsseractContext XsseractContext => XsseractApplication?.XsseractContext;
    protected XsseractApp XsseractApplication => BaseContext.ApplicationContext as XsseractApp;
    protected ToolbarFragment Toolbar
    {
      get
      {
        if (null != toolbar)
        {
          return toolbar;
        }

        toolbar = SupportFragmentManager.FindFragmentById(Resource.Id.toolbar) as ToolbarFragment;
        return toolbar;
      }
    }

    public override void SetContentView(int layoutResID)
    {
      base.SetContentView(layoutResID);

      var host = FindViewById(Resource.Id.optionsMenuHost);
      if (null != host)
      {
        Toolbar.EnableOptionsMenu(host);
      }
    }

    public override bool OnContextItemSelected(IMenuItem item)
    {
      switch(item.ItemId)
      {
        case Resource.Id.tutorial:
          XsseractContext.LogEvent(AppTrackingEvents.Tutorial);

          var intent = new Intent(this, typeof(HelpActivity));
          intent.PutExtra(HelpActivity.Constants.FinishOnClose, true);
          StartActivity(intent);
          break;
        case Resource.Id.rateUs:
          StartRateApplicationActivity();
          break;
        case Resource.Id.feedback:
          Intent feedbackIntent = new Intent(Intent.ActionSendto, Uri.FromParts("mailto", XsseractContext.Settings.FeedbackEmailAddress, null));
          feedbackIntent.PutExtra(Intent.ExtraSubject, Resources.GetString(Resource.String.text_FeedbackEmailSubject));

          StartActivity(Intent.CreateChooser(feedbackIntent, Resources.GetString(Resource.String.text_FeedbackChooserTitle)));
          break;
        case Resource.Id.about:
          StartActivity(typeof(AboutActivity));
          break;
      }
      return base.OnContextItemSelected(item);
    }

    public override void OnCreateContextMenu(IContextMenu menu, View v, IContextMenuContextMenuInfo menuInfo)
    {
      MenuInflater.Inflate(Resource.Layout.OptionsMenu, menu);
    }

    public override bool OnKeyDown([GeneratedEnum] Keycode keyCode, KeyEvent e)
    {
      if (keyCode == Keycode.Menu)
      {
        var host = FindViewById(Resource.Id.optionsMenuHost);
        if (null != host)
        {
          OpenContextMenu(host);
          return true;
        }
      }
      return base.OnKeyDown(keyCode, e);
    }

    public void LogInfo(string message)
    {
      XsseractContext.LogInfo(message);
    }

    public void LogInfo(string format, params object[] args)
    {
      XsseractContext.LogInfo(format, args);
    }

    public void LogDebug(string message)
    {
      XsseractContext.LogDebug(message);
    }

    public void LogDebug(string format, params object[] args)
    {
      XsseractContext.LogDebug(format, args);
    }

    public void LogError(Exception e)
    {
      XsseractContext.LogError(e);
    }

    protected override void OnCreate(Bundle savedInstanceState)
    {
      base.OnCreate(savedInstanceState);

      RequestedOrientation = ScreenOrientation.Portrait;
    }

    protected void DisplayAlert(string message, Action callback)
    {
      new AlertDialog.Builder(this)
        .SetTitle(Resource.String.text_AlertTitle)
        .SetMessage(message)
        .SetPositiveButton(Android.Resource.String.Ok, delegate { callback?.Invoke(); })
        .Show();
    }

    protected void DisplayAlert(string message, Func<Task> callback)
    {
      new AlertDialog.Builder(this)
        .SetTitle(Resource.String.text_AlertTitle)
        .SetMessage(message)
        .SetPositiveButton(Android.Resource.String.Ok,
          async (sender, e) =>
                {
                  if (null != callback)
                  {
                    await callback();
                  }
                })
        .Show();
    }

    protected void DisplayError(Exception e)
    {
      DisplayError(e, null);
    }

    protected void DisplayError(Exception exception, Action dismissDelegate)
    {
      string message = $"{exception.Message}{Environment.NewLine}{exception.StackTrace[0]}";

      new AlertDialog.Builder(this)
        .SetTitle(Resource.String.text_ErrorTitle)
        .SetMessage(message)
        .SetPositiveButton(Android.Resource.String.Ok, (sender, e) => dismissDelegate?.Invoke())
        .Show();
    }

    protected void DisplayError(string error)
    {
      new AlertDialog.Builder(this)
        .SetTitle(Resource.String.text_ErrorTitle)
        .SetMessage(error)
        .SetPositiveButton(Android.Resource.String.Ok, (IDialogInterfaceOnClickListener)null)
        .Show();
    }

    protected void DisplayProgress(string message)
    {
      if (null != progressDialog)
      {
        throw new InvalidOperationException("A background operation is already in progress.");
      }

      progressDialog = new ProgressDialog(this);
      progressDialog.SetCancelable(false);
      progressDialog.SetMessage(message);
      progressDialog.Show();
    }

    protected void HideProgress()
    {
      progressDialog.Hide();
      progressDialog = null;
    }

    protected void StartRateApplicationActivity()
    {
      try
      {
        XsseractContext.LogEvent(AppTrackingEvents.RateNow);

        var intent = new Intent(Intent.ActionView, Uri.Parse("market://details?id=" + PackageName));
        StartActivity(intent);
      }
      catch(ActivityNotFoundException)
      {
        StartActivity(new Intent(Intent.ActionView, Uri.Parse("https://play.google.com/store/apps/details?id=" + PackageName)));
      }
    }
  }
}
