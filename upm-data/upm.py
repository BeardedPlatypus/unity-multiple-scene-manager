import argparse
import json
import shutil

from dataclasses import dataclass
from pathlib import Path
from typing import List, Optional


@dataclass
class Mapping:
    src: Path
    dst: Path
    metadata: Path

    @property
    def _metadata_dst(self) -> Path:
        return Path(str(self.dst) + ".meta")

    @classmethod
    def from_dict(cls, d: dict, repository_directory: Path):
        src = repository_directory / Path(d["from"])
        dst = Path(d["to"])

        if "metadata" in d:
            metadata = repository_directory / Path(d["metadata"]) / (src.name + ".meta")
        else:
            metadata = Path(str(src) + ".meta")

        return cls(src=src, dst=dst, metadata=metadata)

    def _copy_file_to(self, dst: Path) -> None:
        shutil.copy(self.src, dst / self.dst)
        shutil.copy(self.metadata, dst / self._metadata_dst)

    def _copy_dir_to(self, dst: Path) -> None:
        shutil.copy(self.metadata, dst / self._metadata_dst)
        shutil.copytree(self.src, dst / self.dst, dirs_exist_ok=True)

    def copy_to(self, dst: Path) -> None:
        if self.src.is_file():
            self._copy_file_to(dst)
        elif self.src.is_dir():
            self._copy_dir_to(dst)
        else:
            raise Exception(f"src, {self.src}, is neither a file nor a directory.")


def _load_manifest(repository_directory: Path) -> List[Mapping]:
    manifest_path = repository_directory / "upm-data" / "manifest.json"
    with manifest_path.open("r") as f:
        manifest_data = json.load(f)

    return [ Mapping.from_dict(d, repository_directory) for d in manifest_data["mappings"] ]


def _validate_file_mapping(mapping: Mapping) -> None:
    if not mapping.metadata.is_file():
        raise Exception(f"metadata file, {mapping.metadata}, is not a file.")


def _validate_dir_mapping(mapping: Mapping) -> None:
    if not mapping.metadata.is_file():
        raise Exception(f"metadata file, {mapping.metadata}, is not a file.")


def verify(repository_directory: Path) -> None:
    mappings = _load_manifest(repository_directory)

    for mapping in mappings:
        if mapping.src.is_file():
            _validate_file_mapping(mapping)
        elif mapping.src.is_dir():
            _validate_dir_mapping(mapping)
        else:
            raise Exception(f"src, {mapping.src}, is not a file or directory.")


def prepare(repository_directory: Path) -> None:
    dst = repository_directory / "upm-package"
    mappings = _load_manifest(repository_directory)

    for mapping in mappings:
        mapping.copy_to(dst)


def _parse_arguments():
    parser = argparse.ArgumentParser()
    parser.add_argument("action", help="The type of action to perform. Should be either 'verify' or 'prepare'")
    parser.add_argument("repository_root", help="Path to the root of the repository.")
    return parser.parse_args()


if __name__ == "__main__":
    args = _parse_arguments()
    repo_path = Path(args.repository_root)

    if args.action == "verify":
        verify(repo_path)
    elif args.action == "prepare":
        prepare(repo_path)
