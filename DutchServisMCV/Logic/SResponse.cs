using System;


namespace DutchServisMCV.Logic
{
    struct SResponse
    {
        public SResponse(bool good, string message)
        {
            Good = good;
            Message = message;
        }
        public bool Good { get; }
        public string Message { get; }
    }
}
