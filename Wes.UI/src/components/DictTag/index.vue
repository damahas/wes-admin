<template>
  <div>
    <template v-for="item in matchedOptions" :key="item.value">
      <el-tag
        v-if="shouldRenderTag(item)"
        :disable-transitions="true"
        :type="item.elTagType"
        :class="item.elTagClass"
      >
        {{ item.label }}
      </el-tag>
      <span v-else :class="item.elTagClass">
        {{ item.label }}
      </span>
    </template>
    <template v-if="showUnmatched && showValue">
      {{ unmatchArray.join(' ') }}
    </template>
  </div>
</template>

<script setup>
import { computed, ref } from 'vue';

const props = defineProps({
  options: {
    type: Array,
    default: () => [],
  },
  value: [Number, String, Array],
  showValue: {
    type: Boolean,
    default: true,
  },
  separator: {
    type: String,
    default: ',',
  },
});

const values = computed(() => {
  if (props.value === null || props.value === undefined || props.value === '') {
    return [];
  }
  return Array.isArray(props.value)
    ? props.value.map(String)
    : String(props.value).split(props.separator);
});

const matchedOptions = computed(() => {
  return props.options.filter(option => values.value.includes(String(option.value)));
});

const unmatchArray = ref([]);

const showUnmatched = computed(() => {
  unmatchArray.value = [];
  if (!props.value || props.options.length === 0) {
    return false;
  }
  const hasUnmatched = values.value.some(item =>
    !props.options.some(option => String(option.value) === item)
  );
  if (hasUnmatched) {
    unmatchArray.value = values.value.filter(item =>
      !props.options.some(option => String(option.value) === item)
    );
  }
  return hasUnmatched;
});

function shouldRenderTag(item) {
  const hasType = item.elTagType && item.elTagType !== 'default' && item.elTagType !== '';
  const hasClass = item.elTagClass && item.elTagClass !== '';
  return hasType || hasClass;
}
</script>

<style scoped>
.el-tag + .el-tag,
.el-tag + span,
span + .el-tag {
  margin-left: 8px;
}
</style>
