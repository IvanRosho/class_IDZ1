namespace IDZ1_library
{
    internal interface FileDZZ
    {
        uint width
        {
            get;
            set;
        }
        uint height
        {
            get;
            set;
        }
        uint kadri
        {
            get;
            set;
        }
        uint bytes_per_frame
        {
            get;
        }
        void LoadFile(string path);
        int[,] LoadFrameSpectral(int k, int lg=0, int pg=0);
        int[,] LoadFrameFilm(int k, int lg = 0, int pg = 0);
    }
}
