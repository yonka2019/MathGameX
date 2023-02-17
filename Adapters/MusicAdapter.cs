using Android.Content;
using Android.Views;
using Android.Widget;
using System.Collections.Generic;

namespace MathGame.Adapters
{
    class MusicAdapter : BaseAdapter<Song>
    {
        private Context context;
        private List<Song> songs;

        public List<Song> GetList()
        {
            return songs;
        }

        public MusicAdapter(Context context, List<Song> songs)
        {
            this.context = context;
            this.songs = songs;
        }

        public override Song this[int position] => songs[position];

        public override int Count => songs.Count;

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            LayoutInflater layoutInflater = ((MusicSelectorActivity)context).LayoutInflater;
            View view = layoutInflater.Inflate(Resource.Layout.music_layout, parent, false);

            TextView tvSong = view.FindViewById<TextView>(Resource.Id.musicl_musicName);
            Song _song = songs[position];

            if (_song != null)
                tvSong.Text = _song.Name;

            return view;
        }
    }
}