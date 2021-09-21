import re
import os
import json
import urllib.request
import sys

# Check the version from GitHub release
print("Getting version from sementic release")

BUILD_VERSION = sys.argv[1]
print(f'New version: \n{BUILD_VERSION}\n')


def update_csproj_verison(project_name, version):
    assembly_file = os.path.join(os.getcwd(), 'src', 'Honeybee.UI', f'{project_name}.csproj')

    with open(assembly_file, encoding='utf-8', mode='r') as csFile:
        s = csFile.read()
    with open(assembly_file, encoding='utf-8', mode='w') as f:
        s = re.sub(r"(?<=\SVersion\>)\S+(?=\<\/)", version, s)
        print(f"Update {project_name} AssemblyVersion to: {version}")
        f.write(s)

    pass


# update the version
update_csproj_verison('Honeybee.UI', BUILD_VERSION)
update_csproj_verison('Honeybee.UI.Rhino', BUILD_VERSION)import re
import os
import json
import urllib.request
import sys

# Check the version from GitHub release
print("Getting version from sementic release")

BUILD_VERSION = sys.argv[1]
print(f'New version: \n{BUILD_VERSION}\n')


def update_csproj_verison(project_name, version):
    assembly_file = os.path.join(os.getcwd(), 'src', 'Honeybee.UI', f'{project_name}.csproj')

    with open(assembly_file, encoding='utf-8', mode='r') as csFile:
        s = csFile.read()
    with open(assembly_file, encoding='utf-8', mode='w') as f:
        s = re.sub(r"(?<=\SVersion\>)\S+(?=\<\/)", version, s)
        print(f"Update {project_name} AssemblyVersion to: {version}")
        f.write(s)

    pass


# update the version
update_csproj_verison('Honeybee.UI', BUILD_VERSION)
update_csproj_verison('Honeybee.UI.Rhino', BUILD_VERSION)import re
import os
import json
import urllib.request
import sys

# Check the version from GitHub release
print("Getting version from sementic release")

BUILD_VERSION = sys.argv[1]
print(f'New version: \n{BUILD_VERSION}\n')


def update_csproj_verison(project_name, version):
    assembly_file = os.path.join(os.getcwd(), 'src', 'Honeybee.UI', f'{project_name}.csproj')

    with open(assembly_file, encoding='utf-8', mode='r') as csFile:
        s = csFile.read()
    with open(assembly_file, encoding='utf-8', mode='w') as f:
        s = re.sub(r"(?<=\SVersion\>)\S+(?=\<\/)", version, s)
        print(f"Update {project_name} AssemblyVersion to: {version}")
        f.write(s)

    pass


# update the version
update_csproj_verison('Honeybee.UI', BUILD_VERSION)
update_csproj_verison('Honeybee.UI.Rhino', BUILD_VERSION)import re
import os
import json
import urllib.request
import sys

# Check the version from GitHub release
print("Getting version from sementic release")

BUILD_VERSION = sys.argv[1]
print(f'New version: \n{BUILD_VERSION}\n')


def update_csproj_verison(project_name, version):
    assembly_file = os.path.join(os.getcwd(), 'src', 'Honeybee.UI', f'{project_name}.csproj')

    with open(assembly_file, encoding='utf-8', mode='r') as csFile:
        s = csFile.read()
    with open(assembly_file, encoding='utf-8', mode='w') as f:
        s = re.sub(r"(?<=\SVersion\>)\S+(?=\<\/)", version, s)
        print(f"Update {project_name} AssemblyVersion to: {version}")
        f.write(s)

    pass


# update the version
update_csproj_verison('Honeybee.UI', BUILD_VERSION)
update_csproj_verison('Honeybee.UI.Rhino', BUILD_VERSION)