import subprocess
import os
import urllib.request
import json
import re

NUGET_API_KEY = os.getenv('NUGET_API_KEY')

# print("Getting Honeybee.UI.dll version")
# outputs = subprocess.check_output("powershell (Get-Item src/Honeybee.UI/bin/Release/Honeybee.UI.dll).VersionInfo.ProductVersion")
# BUILD_VERSION = str(outputs)[2:-5]
# print("raw output of version checking: " + str(outputs))
# print("cleaned new build version: " + BUILD_VERSION)

print("Getting version from sementic release")
api = 'https://api.github.com/repos/ladybug-tools/honeybee-ui-dotnet/releases/latest'
with urllib.request.urlopen(api) as r:
    data = json.loads(r.read())
    BUILD_VERSION = data['tag_name'][1:]

print("cleaned new build version: " + BUILD_VERSION)

# Update the version
assembly_file = os.path.join(os.getcwd(), 'src', 'Honeybee.UI', 'Properties', 'AssemblyInfo.cs')
with open(assembly_file, "r") as csFile:
    s = csFile.read()
with open(assembly_file, 'w') as f:
    s = re.sub(r"(?<=assembly: AssemblyVersion\(\")\S+(?=\"\))", BUILD_VERSION + '.*', s)
    print(f"Update AssemblyVersion to: {BUILD_VERSION}")
    f.write(s)

# pack nuget
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

