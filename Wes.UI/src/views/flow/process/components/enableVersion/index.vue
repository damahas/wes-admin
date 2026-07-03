<template>
  <el-switch
    size="small"
    :model-value="switchValue"
    :disabled="switchValue"
    :title="switchValue ? t('flow.designer.versionEnabled') : t('flow.designer.enableVersionDesc')"
    @change="handleChange"
  >
  </el-switch>
</template>

<script setup>
import { ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import { useVersion } from "@/api/flow/process";

const { t } = useI18n();

const props = defineProps({
  value: String,
});

const handleChange = (val) => {
  if (val) {
    useVersion(props.value).then((res) => {});
    emit("updateValue", '');
  }
};

const emit = defineEmits(["updateValue"]);

const switchValue = ref(false);

watch(
  () => props.value,
  (val) => {
    if (!val) {
      switchValue.value = true;
    } else {
      switchValue.value = false;
    }
  }
);
</script>
