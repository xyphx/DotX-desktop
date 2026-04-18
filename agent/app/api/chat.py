from fastapi import APIRouter

router = APIRouter(prefix="/chat", tags=["chat"])


@router.get("/")
def chat_root() -> dict[str, str]:
    return {"message": "chat route placeholder"}
