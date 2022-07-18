using Bogus;
using Rtrw.Client.Wasm.FakeData.Bogus;
using Rtrw.Client.Wasm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rtrw.Client.Wasm.Services
{
    public interface IDummyService
    {
        List<Post> GenerateFakePost(int count);
        List<Post> GenerateFakePost(int count, string kelurahan);
    }
    public class DummyService : IDummyService
    {
        private readonly Dummy dummy = new();
        public List<Post> GenerateFakePost(int count)
        {
            return dummy.GenerateFakePost().GenerateBetween(0,count);
        }

        public List<Post> GenerateFakePost(int count, string kelurahan)
        {
            return dummy.GenerateFakePost(kelurahan).GenerateBetween(0, count);
        }
    }
}
