class InMemoryStore:
    def __init__(self) -> None:
        self._items: list[str] = []

    def add(self, value: str) -> None:
        self._items.append(value)

    def all(self) -> list[str]:
        return list(self._items)
