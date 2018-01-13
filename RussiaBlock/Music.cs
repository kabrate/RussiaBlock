using Microsoft.DirectX.AudioVideoPlayback;
using System;
using Microsoft.DirectX;
using Microsoft.DirectX.DirectSound;
using System.Windows.Forms;
using RussiaBlock;

public static class Music
{
    public static Device sdevice = new Device();//音效
    public static Audio music;//实例
    

    public static void PlaySound(string file_path)
    {
        try
        {
            sdevice.SetCooperativeLevel(MainForm.ActiveForm, Microsoft.DirectX.DirectSound.CooperativeLevel.Normal);
            SecondaryBuffer buf;
            buf = new SecondaryBuffer(file_path, sdevice);
            buf.Play(0, BufferPlayFlags.Default);
            

        }
        catch
        {
            MessageBox.Show("音效错误");
        }
    }//播放音效
    public static double SaveMusic()//记录音乐播放的位置
    {
        
        if (music == null)
            return 0;
        else
            return music.CurrentPosition;
        
            
    }
    public static void StopMusic()//停止播放音乐
    {
        if (music == null)
            return;
        else
        music.Stop();
    }
    public static void PlayMusic(string file_path,double currentposition)
    {
        try
        {
            if (music == null)
                return;
            else
            {
                music.CurrentPosition = currentposition;
                //music = Audio.FromFile(file_path);
                music.Ending += new EventHandler(music_end);
                music.Play();
            }
        }
        catch
        {
            MessageBox.Show("音乐错误");
        }
    }//播放音乐
    public static void PlayMusic(string file_path)
    {
        try
        {
            if (music != null)
            {
                music.Stop();
                music = null;
            }

            music = Audio.FromFile(file_path);
            music.Ending += new EventHandler(music_end);
            music.Play();
        }
        catch
        {
            MessageBox.Show("音乐错误");
        }
    }
    public static void music_end(object sender, EventArgs e)
    {
        if (music == null)
            return;

        if (music.Stopped)
            return;

        try
        {
            music.CurrentPosition = 0;
        }
        catch { }
    }//音乐结束后的处理
}