<template>
  <div>
    <el-select
      v-model="selectUserId"
      filterable
      remote
      clearable
      style="width: 100%"
      :remote-method="handleUserList"
      :loading="loading"
      @focus="handleUserList('')"
    >
      <el-option
        v-for="item in options"
        :key="item.userId"
        :label="item.userName"
        :value="item.userId"
      />
    </el-select>
  </div>
</template>

<script setup>
import { ref, computed } from "vue";
import { listUser } from "@/api/system/user";

const props = defineProps({
  userId: {
    type: [Number, String],
    default: null,
  },
  userName: {
    type: String,
    default: "",
  },
});

const emit = defineEmits(["update:userId", "update:userName"]);

const companys = ref([]);
const loading = ref(false);

const selectUserId = computed({
  get() {
    return props.userId;
  },
  set(val) {
    emit("update:userId", val);
    emit(
      "update:userName",
      companys.value.find((p) => p.userId == val)?.userName || ""
    );
  },
});

const options = computed(() => {
  if (!props.userId || companys.value.find((p) => p.userId == props.userId)) {
    return companys.value;
  }
  return [
    { userId: props.userId, userName: props.userName },
    ...companys.value,
  ];
});

function handleUserList(query) {
  const param = {
    pageNum: 1,
    pageSize: 20,
    params: {
      userName: query,
    },
  };
  loading.value = true;
  listUser(param).then((response) => {
    loading.value = false;
    companys.value = response.rows;
  });
}
</script>
