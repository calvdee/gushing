rmdir /S Gushing/packages
rmdir /S Gushing.Tests/packages

mkdir Gushing/packages
mkdir Gushing.Tests/packages

nuget restore Gushing/packages.config -PackagesDirectory "Gushing/packages"
nuget restore Gushing.Tests/packages.config -PackagesDirectory "Gushing.Tests/packages"