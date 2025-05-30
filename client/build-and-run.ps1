function Invoke-BuildAndRunContainer {
    param (
        [string]$ImageName = "vismaenterpriseprise.hometask.client",
        [string]$ContainerName = "visma_hometask_client_container"
    )

    if ($(docker ps -a -q -f name=$ContainerName)) {
        docker stop $ContainerName
        docker rm $ContainerName
    }

    if ($(docker images -q $ImageName)) {
        docker rmi $ImageName
    }

    docker build -t $ImageName -f .\Dockerfile .
    docker run -d -p 8081:4200 --name $ContainerName $ImageName
}

Invoke-BuildAndRunContainer
