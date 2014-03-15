namespace Galssoft.VKontakteWM.Components.Common.Interfaces
{
    interface ISelfDeserializer
    {
        /// <summary>
        /// Собственный код сериализации данных 
        /// </summary>
        /// <param name="folderName">Папка</param>
        /// <param name="dataName">Файл данных</param>
        void Deserialize(string folderName, string dataName);

        /// <summary>
        /// Собственный код десериализации данных 
        /// </summary>
        /// <param name="folderName">Папка</param>
        /// <param name="dataName">Файл данных</param>
        void Serialize(string folderName, string dataName);
    }
}
