using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Storage;
using System.IO;
using System.ComponentModel;

namespace BugTrackerProject.Storage
{
    public class FirebaseFileStorage : IFirebaseFileStorage
    {
        private readonly FirebaseStorageSettings _settings;

        public FirebaseFileStorage(IOptions<FirebaseStorageSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task<string> Upload (byte[] fileBytes, string fileName)
        {

            var auth = new FirebaseAuthProvider(new FirebaseConfig(_settings.ApiKey));
            var a = await auth.SignInWithEmailAndPasswordAsync(_settings.Email, _settings.Password);

            var task = new FirebaseStorage(
                _settings.Bucket,
                new FirebaseStorageOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                    ThrowOnCancel = true // when you cancel the upload, exception is thrown. By default no exception is thrown
                })
                .Child("Screenshots")
                .Child(fileName)
                .PutAsync(new MemoryStream(fileBytes));

            var percentage = "";

            task.Progress.ProgressChanged += (s, e) => {
                    percentage = $"Progress: {e.Percentage} %";
                };

            // cancel the upload
            // cancellation.Cancel();

            return await task;
        }


        public async void Delete(string fileName)
        {

            var auth = new FirebaseAuthProvider(new FirebaseConfig(_settings.ApiKey));
            var a = await auth.SignInWithEmailAndPasswordAsync(_settings.Email, _settings.Password);

            var task = new FirebaseStorage(
                _settings.Bucket,
                new FirebaseStorageOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                    ThrowOnCancel = true // when you cancel the upload, exception is thrown. By default no exception is thrown
                })
                .Child("Screenshots")
                .Child(fileName)
                .DeleteAsync();

        }
    }
}
