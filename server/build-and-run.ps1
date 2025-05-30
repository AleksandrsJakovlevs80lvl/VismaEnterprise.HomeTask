function Invoke-ApplyDbMigration
{
    dotnet ef database update --project .\\VismaEnterprise.HomeTask.Infrastructure\\VismaEnterprise.HomeTask.Infrastructure.csproj
}

function Invoke-BuildAndRunContainer {
    param (
        [string]$ImageName = "vismaenterpriseprise.hometask.web",
        [string]$ContainerName = "visma_hometask_web_container"
    )

    if ($(docker ps -a -q -f name=$ContainerName)) {
        docker stop $ContainerName
        docker rm $ContainerName
    }

    if ($(docker images -q $ImageName)) {
        docker rmi $ImageName
    }

    docker build -t $ImageName -f .\VismaEnterprise.HomeTask.Web\Dockerfile .
    docker run -d -p 8080:8080 --name $ContainerName $ImageName
}

Invoke-ApplyDbMigration
Invoke-BuildAndRunContainer