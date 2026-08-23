from fastapi import FastAPI
from pydantic import BaseModel
from sentence_transformers import SentenceTransformer

from huggingface_hub import login

access_token = ""
login(token=access_token)


# Your existing code with a small modification
model = SentenceTransformer(
    "sentence-transformers/paraphrase-multilingual-mpnet-base-v2"
)

# Set the attribute on the tokenizer correctly
model.tokenizer.clean_up_tokenization_spaces = True


app = FastAPI()

# model = SentenceTransformer('paraphrase-multilingual-mpnet-base-v2')
# multi-qa-MiniLM-L6-cos-v1
# paraphrase-multilingual-mpnet-base-v2
class QueryInput(BaseModel):
    searchquery: str

class EntityInput(BaseModel):
    entity: str

@app.post("/embedQuery")
def embed_Query(item: QueryInput):
    embedding = model.encode(item.searchquery).tolist()
    return {"embedding": embedding}

@app.post("/embedEntity")
def embed_Entity(item: EntityInput):
    print(item)
    embedding = model.encode(item.entity).tolist()
    return {"embedding": embedding}


# if __name__ == "__main__":
#     uvicorn.run(app, host="0.0.0.0", port=8000)
# uvicorn main:app --reload  <--run this 