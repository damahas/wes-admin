#!/bin/bash
set -e

# ============================================
# WesAdmin Docker 构建脚本
# 步骤: 前端构建 → 后端发布 → 打包镜像
# ============================================

PROJECT_ROOT="$(cd "$(dirname "$0")" && pwd)"
IMAGE_NAME="${IMAGE_NAME:-wes-admin}"
IMAGE_TAG="${IMAGE_TAG:-latest}"

# 计时工具
BUILD_START=$(date +%s)
format_duration() {
    local secs="$1"
    printf "%dm%ds" $((secs / 60)) $((secs % 60))
}

echo "========================================"
echo "  WesAdmin Docker Build"
echo "  镜像: ${IMAGE_NAME}:${IMAGE_TAG}"
echo "  项目: ${PROJECT_ROOT}"
echo "  开始: $(date '+%Y-%m-%d %H:%M:%S')"
echo "========================================"

# ---------- 1. 编译前端 ----------
echo ""
echo "[1/3] 编译 Vue 前端..."
STEP_START=$(date +%s)

cd "$PROJECT_ROOT/Wes.UI"

echo "  → npm install..."
npm install --silent

echo "  → npm run build..."
npm run build

STEP_END=$(date +%s)
FE_DURATION=$((STEP_END - STEP_START))
echo "  ✓ 前端编译完成 (耗时: $(format_duration $FE_DURATION))"

# ---------- 2. 发布后端 ----------
echo ""
echo "[2/3] 发布 .NET 后端..."
STEP_START=$(date +%s)

cd "$PROJECT_ROOT"

PUBLISH_DIR="$PROJECT_ROOT/Wes.WebApi/bin/Release/net10.0/publish"

echo "  → dotnet publish..."
dotnet publish "$PROJECT_ROOT/Wes.WebApi/Wes.WebApi.csproj" \
    -c Release \
    -o "$PUBLISH_DIR"

# 验证产物
if [ ! -f "$PUBLISH_DIR/Wes.WebApi.dll" ]; then
    echo "  ✗ 发布失败: $PUBLISH_DIR/Wes.WebApi.dll 不存在"
    exit 1
fi

STEP_END=$(date +%s)
BE_DURATION=$((STEP_END - STEP_START))
echo "  ✓ 后端发布完成 (耗时: $(format_duration $BE_DURATION))"

# ---------- 3. 构建 Docker 镜像 ----------
echo ""
echo "[3/3] 打包 Docker 镜像..."
STEP_START=$(date +%s)

docker build \
    -f Wes.WebApi/Dockerfile \
    -t "${IMAGE_NAME}:${IMAGE_TAG}" \
    "$PROJECT_ROOT"

STEP_END=$(date +%s)
DOCKER_DURATION=$((STEP_END - STEP_START))
echo "  ✓ Docker 镜像打包完成 (耗时: $(format_duration $DOCKER_DURATION))"

# ---------- 汇总 ----------
BUILD_END=$(date +%s)
TOTAL=$((BUILD_END - BUILD_START))

echo ""
echo "========================================"
echo "  ✓ 构建完成!"
echo "  镜像: ${IMAGE_NAME}:${IMAGE_TAG}"
echo "========================================"
echo "  耗时统计:"
echo "    前端编译:  $(format_duration $FE_DURATION)"
echo "    后端发布:  $(format_duration $BE_DURATION)"
echo "    Docker:    $(format_duration $DOCKER_DURATION)"
echo "    ─────────────────────"
echo "    总耗时:    $(format_duration $TOTAL)"
echo "========================================"
echo "  运行: docker run -d -p 8088:80 ${IMAGE_NAME}:${IMAGE_TAG}"
