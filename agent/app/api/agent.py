from fastapi import APIRouter

router = APIRouter(prefix="/agent", tags=["agent"])


@router.get("/")
def agent_root() -> dict[str, str]:
    return {"message": "agent route placeholder"}
