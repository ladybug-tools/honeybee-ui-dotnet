import subprocess
import os

NUGET_API_KEY = os.getenv('NUGET_API_KEY')

print("Getting Honeybee.UI.dll version")
outputs = subprocess.check_output("powershell (Get-Item src/Honeybee.UI/bin/Release/Honeybee.UI.dll).VersionInfo.ProductVersion")
BUILD_VERSION = str(outputs)[2:-5]
print("raw output of version checking: " + str(outputs))
print("cleaned new build version: " + BUILD_VERSION)

try:
    nuget_pack = f"nuget pack src/Honeybee.UI/Honeybee.UI.csproj -Properties Configuration=Release -Version {BUILD_VERSION} -Verbosity detailed"
    subprocess.check_output(nuget_pack)
    nuget_push = f"nuget push Honeybee.UI.{BUILD_VERSION}.nupkg -ApiKey {NUGET_API_KEY} -Source https://api.nuget.org/v3/index.json"
    subprocess.check_output(nuget_push)

    nuget_pack = f"nuget pack src/Honeybee.UI/Honeybee.UI.csproj -suffix Rhino -Properties Configuration=Release_Rhino -Version {BUILD_VERSION} -Verbosity detailed"
    subprocess.check_output(nuget_pack)
    nuget_push = f"nuget push Honeybee.UI.{BUILD_VERSION}-Rhino.nupkg -ApiKey {NUGET_API_KEY} -Source https://api.nuget.org/v3/index.json"
    subprocess.check_output(nuget_push)

except Exception as e:
    print(str(e))
    raise Exception(e)

