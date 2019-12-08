using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;

namespace HeroesPowerPlant.RemoteControl.Shared
{
    public static class ServerLocator
    {
        /// <summary>
        /// Attempts to acquire the port to connect to a remote process.
        /// </summary>
        /// <exception cref="FileNotFoundException"><see cref="MemoryMappedFile"/> was not created by the mod loader, in other words, mod loader is not loaded.</exception>
        public static int GetPortOfServer(Process process)
        {
            return GetPortOfServer(process.Id);
        }

        /// <summary>
        /// Attempts to acquire the port to connect to a remote process with a specific id.
        /// </summary>
        /// <exception cref="FileNotFoundException"><see cref="MemoryMappedFile"/> was not created by the mod loader, in other words, mod loader is not loaded.</exception>
        public static int GetPortOfServer(int pid)
        {
            var mappedFile = MemoryMappedFile.OpenExisting(GetMappedFileNameForPid(pid));
            var view = mappedFile.CreateViewStream();
            var binaryReader = new BinaryReader(view);
            return binaryReader.ReadInt32();
        }

        /// <summary>
        /// Registers a specific port for the current process.
        /// </summary>
        /// <returns>A <see cref="MemoryMappedFile"/> which should have lifespan equal to the server.</returns>
        public static MemoryMappedFile RegisterPortForServer(int port)
        {
            int pid = Process.GetCurrentProcess().Id;
            var mappedFile = MemoryMappedFile.CreateOrOpen(GetMappedFileNameForPid(pid), sizeof(int));
            var view = mappedFile.CreateViewStream();
            var binaryWriter = new BinaryWriter(view);
            binaryWriter.Write(port);
            return mappedFile;
        }

        /// <summary>
        /// Returns the name of a memory mapped file given the process ID of a foreign mapped file.
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public static string GetMappedFileNameForPid(int pid)
        {
            return $"HPP-Remote-Control-Server-PID-{pid}";
        }
    }
}
