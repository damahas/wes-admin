<template>
  <div>
    <div class="slider-img" v-show="sliderForm.isShowImg" v-loading="sliderForm.loading">
      <!-- <div class="slider-img" v-loading="loading"> -->
      <img
        class="img-bg"
        v-if="sliderForm.sliderImg.captchaImg"
        :src="sliderForm.sliderImg.captchaImg"
      />
      <img
        class="img-s"
        :style="sliderPosition"
        v-if="sliderForm.sliderImg.sliderImg"
        :src="sliderForm.sliderImg.sliderImg"
      />
    </div>
    <div class="slider-way" ref="sliderWay">
      <div
        class="slider-btn"
        ref="sliderBtn"
        :style="{ left: sliderForm.sliderLeft + 'px' }"
        v-drag="{
          methods: { handleGetCodeImg, handleOk },
          form: sliderForm,
        }"
      >
        <i class="fa fa-angles-right"></i>
      </div>
      <i
        v-if="sliderForm.isSuccess"
        style="success-icon"
        class="fa fa-circle-check success-icon"
      ></i>
    </div>
  </div>
</template>

<script setup>
import { getCodeImg, validCodeImg } from "@/api/login";
import { computed, onMounted, onUnmounted, defineEmits, ref } from "vue";
const emits = defineEmits(["handleOk"]);

defineExpose({
  reset,
});

// const isShowImg = ref(false);
// const isCanMove = ref(true);
// const loading = ref(false);
// const isSuccess = ref(false);
// const sliderLeft = ref(0);
// const sliderImg = ref({
//   captchaImg: "",
//   sliderImg: "",
//   sliderPositionY: 0,
//   code: "",
// });

const sliderForm = ref({
  isShowImg: false,
  isCanMove: true,
  loading: false,
  isSuccess: false,
  sliderLeft: 0,
  sliderImg: {
    captchaImg: "",
    sliderImg: "",
    sliderPositionY: 0,
    code: "",
  },
  moveMaxWidth: 0,
});
const sliderPosition = computed(() => {
  return `top:${sliderForm.value.sliderImg.sliderPositionY}%;left:${sliderForm.value.sliderLeft}px`;
});
const sliderWay = ref();
const sliderBtn = ref();
const vDrag = (el, _sliderForm) => {
  let sliderForm = _sliderForm.value.form;
  el.onmousedown = (e) => {
    _sliderForm.value.methods.handleGetCodeImg();
    if (!sliderForm.isCanMove) return false;
    sliderForm.isShowImg = true;
    sliderForm.isSuccess = false;
    _sliderForm.value.methods.handleOk();
    const disX = e.clientX - el.offsetLeft;
    document.onmousemove = (e) => {
      const l = e.clientX - disX;
      if (l < 0) {
        sliderForm.sliderLeft = 0;
      } else if (l > sliderForm.moveMaxWidth) {
        sliderForm.sliderLeft = sliderForm.moveMaxWidth;
      } else {
        sliderForm.sliderLeft = e.clientX - disX;
      }
    };
    document.onmouseup = (e) => {
      document.onmousemove = null;
      document.onmouseup = null;
      sliderForm.isCanMove = false;

      const left = sliderForm.sliderLeft / sliderWay.value.clientWidth;

      validCodeImg(sliderForm.sliderImg.code, left)
        .then((response) => {
          sliderForm.isShowImg = false;
          sliderForm.sliderLeft = 0;
          sliderForm.isCanMove = true;
          sliderForm.isSuccess = true;
          if (response.code == 200) {
            _sliderForm.value.methods.handleOk({
              code: sliderForm.sliderImg.code,
              positionX: left,
            });
            return;
          }
        })
        .catch((err) => {
          sliderForm.isShowImg = false;
          sliderForm.sliderLeft = 0;
          sliderForm.isCanMove = true;
          _sliderForm.value.methods.handleOk();
        });
    };
    return false;
  };
  el.ontouchstart = (e) => {
    sliderForm.value.methods.handleGetCodeImg();
    if (!_sliderForm.isCanMove) return false;
    sliderForm.isShowImg = true;
    sliderForm.isSuccess = false;
    _sliderForm.value.methods.handleOk();
    const disX = e.targetTouches?.[0].clientX;
    document.ontouchmove = (e) => {
      const l = e.targetTouches?.[0].clientX - disX;
      if (l < 0) {
        sliderForm.sliderLeft = 0;
      } else if (l > sliderForm.moveMaxWidth) {
        sliderForm.sliderLeft = sliderForm.moveMaxWidth;
      } else {
        sliderForm.sliderLeft = e.targetTouches?.[0].clientX - disX;
      }
    };
    document.ontouchend = (e) => {
      document.ontouchmove = null;
      document.ontouchend = null;
      sliderForm.isCanMove = false;

      const left = sliderForm.sliderLeft / sliderWay.value.clientWidth;

      validCodeImg(sliderForm.sliderImg.code, left)
        .then((response) => {
          sliderForm.isShowImg = false;
          sliderForm.sliderLeft = 0;
          sliderForm.isCanMove = true;
          sliderForm.isSuccess = true;
          if (response.code == 200) {
            _sliderForm.value.methods.handleOk({
              code: sliderForm.sliderImg.code,
              positionX: left,
            });
            return;
          }
        })
        .catch((err) => {
          sliderForm.isShowImg = false;
          sliderForm.sliderLeft = 0;
          sliderForm.isCanMove = true;
          _sliderForm.value.methods.handleOk();
        });
    };
    return false;
  };
};

onMounted(() => {
  document.addEventListener("selectstart", disabledSelect);

  sliderForm.value.moveMaxWidth =
    sliderWay.value.clientWidth - sliderBtn.value.clientWidth;
});
onUnmounted(() => {
  document.removeEventListener("selectstart", disabledSelect);
});

function handleOk(val) {
  emits("handleOk", val);
}

function handleGetCodeImg() {
  sliderForm.value.loading = true;
  const width = sliderBtn.value.clientWidth / sliderWay.value.clientWidth;
  const height = sliderBtn.value.clientWidth / 170;
  getCodeImg(width.toFixed(2), height.toFixed(2))
    .then((response) => {
      sliderForm.value.sliderImg = response.data;
      sliderForm.value.isCanMove = true;
      sliderForm.value.loading = false;
    })
    .catch((err) => {
      sliderForm.value.isCanMove = true;
      sliderForm.value.loading = false;
    });
}
function reset() {
  sliderForm.value.isSuccess = false;
}

function disabledSelect(event) {
  event.preventDefault();
}
</script>

<style lang="scss" scoped>
.slider-img {
  width: 100%;
  position: absolute;
  height: 170px;
  top: -180px;
  background-color: var(--bg-card);
  border: 1px solid var(--border-color);

  .img-bg {
    position: absolute;
    width: 100%;
    height: 100%;
  }

  .img-s {
    position: absolute;
    width: 40px;
    height: 40px;
  }
}

.slider-way {
  position: relative;
  height: 32px;
  width: 100%;
  border: 1px solid var(--border-color);
  border-radius: 4px;
  background-color: var(--bg-card);
  .slider-btn {
    position: absolute;
    width: 40px;
    text-align: center;
    cursor: pointer;
    color: #FFFFFF;
    border-radius: 4px;
    background: linear-gradient(135deg, #6BA368 0%, #8CC488 100%);
    line-height: 31px;
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.08), 0 0 0 1px rgba(107, 163, 104, 0.2);

    &:hover {
      background: linear-gradient(135deg, #8CC488 0%, #A5D9A1 100%);
    }

    &:active {
      background: #528A4E;
      transform: scale(0.98);
    }

    i {
      font-size: 14px;
    }
  }
}

.success-icon {
  color: var(--color-success);
  float: right;
  font-size: 18px;
  margin-right: 6px;
  line-height: 32px;
}
</style>
