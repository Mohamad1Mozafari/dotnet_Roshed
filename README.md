docker run -d -p 5432:5432 -e POSTGRES_PASSWORD=MyPassword123! -e POSTGRES_USER=admin -e POSTGRES_DB=TehranRoshed --name postgres postgres:latest



docker run -d -p 6333:6333 -p 6334:6334 -v qdrant_storage:/qdrant/storage --name qdrant qdrant/qdrant



run these:
(terminal 1):
cd PEmbeder
uvicorn main:app --reload 

(terminal 2):
cd RoshedTehran 
dotnet run seeddata 
#(for add sample data to db )


###
http://localhost:6333/dashboard#/collections
(see the qdrant doshbord ) 
